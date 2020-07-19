using ConfigCore.common.models;
using ConfigCore.config;
using Consul;
using DiscoveryCore.common;
using DiscoveryCore.common.interfaces;
using DiscoveryCore.common.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DiscoveryCore.consul
{
	public class ConsulDiscovery : DiscoverySource
	{
		private const string ADDRESS = "http://localhost:8500";

		private readonly IConfig _config;
		private readonly ILogger _logger;

		private ConsulClient _client;
		private RegisterConfiguration _regConfig;
		private ConsulServiceInstance _consulServiceInstance;

		public int _startRetryDelay;
		public int _maxRetryDelay;
		public string _protocol;
		public bool _canRun;
		public string _lastKnownService;
		public List<GatewayURLWatch> _gatewayURLs;

		public ConsulDiscovery(ConfigOptions configOptions, ILogger logger)
		{
			_config = new Config(configOptions);
			_logger = logger;

			(_startRetryDelay, _maxRetryDelay) = Common.GetRetryDelays(_config);

			//getting source address
			string sourceAddress = _config.Get<string>("kumuluzee.discovery.consul.hosts");
			if (string.IsNullOrWhiteSpace(sourceAddress))
				sourceAddress = ADDRESS;

			//creating consul client
			try
			{
				_client = new ConsulClient(c =>
				{
					c.Address = new Uri(sourceAddress);
				});
			}
			catch
			{
				_client = null;
			}

			if (_client == null)
				_logger.LogWarning("Thare was problem creating consul client.");

			_protocol = _config.Get<string>("kumuluzee.discovery.consul.protocol");
			if (string.IsNullOrWhiteSpace(_protocol))
				_protocol = "http";
		}

		public string RegisterService(RegisterOptions options)
		{
			if (_consulServiceInstance != null)
				return "Service is already registered";

			_regConfig = Common.GetServiceRegisterConfiguration(_config, options);
			_consulServiceInstance = new ConsulServiceInstance(_regConfig, isSingleton: options.Singleton);

			_canRun = true;
			Run(_startRetryDelay);

			return _consulServiceInstance.Id.ToString();
		}
		private async void Run(int retryDelayMs)
		{
			if (!_canRun)
				return;

			ExecutionStatus status;
			bool firstRunStatus = false;

			if (_consulServiceInstance.IsRegistered)
			{
				status = await TTLUpdate();
				if(!status.Successful)
					_consulServiceInstance.IsRegistered = false;
			}
			else
			{
				status = await Register();
				if (status.Successful)
				{
					firstRunStatus = true;
					_consulServiceInstance.IsRegistered = true;
				}
			}

			if (status.Successful)
			{
				if (!firstRunStatus)
					await Task.Delay(_regConfig.Discovery.PingInterval);
				Run(_startRetryDelay);
			}
			else
			{
				await Task.Delay(retryDelayMs);
				if (retryDelayMs * 2 > _maxRetryDelay)
					Run(_maxRetryDelay);
				else
					Run(retryDelayMs * 2);
			}
		}
		private async Task<ExecutionStatus> TTLUpdate()
		{
			WriteResult result = null;
			try
			{
				if(_canRun)
					result = await _client.Agent.UpdateTTL(
									$"service:{_consulServiceInstance.Id.ToString()}",
									$"serviceid={_consulServiceInstance.Id} time={DateTime.Now}",
									TTLStatus.Pass
								);
			}
			catch
			{
				return ExecutionStatus.Bad();
			}

			if (result == null || result.StatusCode != HttpStatusCode.OK)
			{
				return ExecutionStatus.Bad();
			}

			return ExecutionStatus.Good();
		}
		private async Task<ExecutionStatus> Register()
		{
			if (_consulServiceInstance.IsSingleton && await IsServiceRegistred())
				return ExecutionStatus.Bad();		

			var agent = new AgentServiceRegistration()
			{
				ID = _consulServiceInstance.Id.ToString(),
				Name = _consulServiceInstance.Name,
				Port = _regConfig.Server.Port,
				Tags = new string[] { _protocol, _consulServiceInstance.VersionTag },
				Address = string.IsNullOrWhiteSpace(_regConfig.Server.Address) ? null : _regConfig.Server.Address,
				Check = new AgentCheckRegistration()
				{
					TTL = TimeSpan.FromMilliseconds(_regConfig.Discovery.TTL),
					DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10),
					Status = HealthStatus.Passing,
					ServiceID = _consulServiceInstance.Id.ToString(),
				}
			};

			try
			{
				var result = await _client.Agent.ServiceRegister(agent);
			}
            catch
            {
				return ExecutionStatus.Bad();
			}

			if (result.StatusCode != HttpStatusCode.OK)
				return ExecutionStatus.Bad();

			return ExecutionStatus.Good();
		}
		private async Task<bool> IsServiceRegistred()
		{
			var res = await _client.Health.Service(_consulServiceInstance.Id.ToString(), "", true);
			if (res == null || res.StatusCode != System.Net.HttpStatusCode.OK)
			{
				//Logging
				return false;
			}
			return true;
		}

		public async Task<string> DiscoverService(DiscoverOptions options)
		{
			options.CompleteDiscoverOptions();

			var searchServiceName = $"{options.Environment}-{options.ServiceName}"; 
			var res = await _client.Health.Service(searchServiceName, "", true);
			if(res.StatusCode != HttpStatusCode.OK)
			{
				if (!string.IsNullOrWhiteSpace(_lastKnownService))
				{
					_logger.LogWarning("Unsuccessful service discovery. Last known service is used.");
					return _lastKnownService;
				}
				_logger.LogError("Unsuvessful service discovery. An empty string is returned");
				return "";
			}

			var allDiscoveries = res.Response.Select(e => new DiscoveredService(e)).Where(e => e.Version != null).ToList();

			foreach (var discovery in allDiscoveries) 
			{
				var watchNamespace = $"/environments/{options.Environment}/services/{options.ServiceName}/{discovery.Version}";

				if(_gatewayURLs.Where(e => e.Id == watchNamespace).Any() == false)
				{
					_logger.LogInformation($"Creating a watch for {watchNamespace}");
					var key = "gatewayUrl";


					_gatewayURLs.Add(new GatewayURLWatch()
					{
						Id = watchNamespace,
						URL = _config.Get<string>(key)
					});

					_config.Subscribe(key, (string val) =>
					{
						foreach(var gatewayURL in _gatewayURLs)
						{
							if(gatewayURL.Id == watchNamespace)
							{
								//logging
								gatewayURL.URL = val;
							}
						}
					});
				}	
			}

			var service = Common.GetRandomServiceInstance(allDiscoveries, _gatewayURLs, options, _lastKnownService);
			//TODO: check if there was any error getting random service
			_lastKnownService = service;
			return service;
		}

		public async Task<ExecutionStatus> UnregisterService()
		{
			ExecutionStatus status;

			if(_consulServiceInstance == null)
			{
				status = ExecutionStatus.Bad();
				status.Message = $"No service is registered";
				return status;
			}
			_canRun = false;

			var result = await _client.Agent.ServiceDeregister(_consulServiceInstance.Id.ToString());

			if(result.StatusCode == HttpStatusCode.OK)
			{
				status = ExecutionStatus.Good();
				status.Message = $"Service with id {_consulServiceInstance.Id.ToString()} was successfully unregistred.";
			}
			else
			{
				status = ExecutionStatus.Bad();
				status.Message = $"There were some problems unregistering service with id {_consulServiceInstance.Id.ToString()}.";
			}

			return status;
		}
	}

	internal class ConsulServiceInstance
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string VersionTag { get; set; }
		public bool IsSingleton { get; set; }
		public bool IsRegistered { get; set; }

		public ConsulServiceInstance(RegisterConfiguration regConfig, bool isSingleton = true)
		{
			Id = Guid.NewGuid();
			Name = $"{regConfig.EnvName}-{regConfig.Name}";
			VersionTag = $"version={regConfig.Version}";
			IsSingleton = isSingleton;
		}
	}
}
