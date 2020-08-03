using ConfigCore.common.models;
using ConfigCore.config;
using Consul;
using DiscoveryCore.common;
using DiscoveryCore.common.interfaces;
using DiscoveryCore.common.models;
using Microsoft.Extensions.Logging;
using Nancy.Diagnostics;
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
		private ConsulServiceInstance _consulServiceInstance;

		public int _startRetryDelay;
		public int _maxRetryDelay;
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
		}


		public string RegisterService(RegisterOptions options)
		{
			if (_consulServiceInstance != null)
				return "Service is already registered";

			var regConfig = Common.GetServiceRegisterConfiguration(_config, options);
			_consulServiceInstance = new ConsulServiceInstance(regConfig);

			_canRun = true;
			Run(regConfig.Discovery.PingInterval * 1000);

			return _consulServiceInstance.Id;
		}
		private async void Run(int pingIntervalMs)
		{
			if (!_canRun)
				return;

			//if service is already registered
			if (_consulServiceInstance.IsRegistered)
				await SendHeartbeat();
			else
				await Register(_startRetryDelay);

			await Task.Delay(pingIntervalMs);
			Run(pingIntervalMs);
		}
		private async Task SendHeartbeat()
		{
			try
			{
				var result = await _client.Agent.UpdateTTL(
								$"service:{_consulServiceInstance.Id}",
								$"serviceid={_consulServiceInstance.Id} time={DateTime.Now}",
								TTLStatus.Pass
							);

				if (result == null || result.StatusCode != HttpStatusCode.OK)
					throw new Exception();
			}
			catch
			{
				//TODO: Logging
				_consulServiceInstance.IsRegistered = false;
				await Register(_startRetryDelay);
			}

		}
		private async Task Register(int retryDelayMs)
		{
			if (_consulServiceInstance.IsSingleton && await IsServiceRegistered())
            {
				_logger.LogInformation("Service is already registred.");
				return;
            }

			if (_client?.Agent == null)
			{
				_logger.LogWarning("Consul not initialized.");
				return;
			}

			var agent = new AgentServiceRegistration()
			{
				ID = _consulServiceInstance.Id,
				Name = _consulServiceInstance.ServiceName,
				Port = _consulServiceInstance.Port,
				Tags = new string[] { _consulServiceInstance.Protocol, _consulServiceInstance.VersionTag },
				Address = _consulServiceInstance.Address,
				Check = new AgentCheckRegistration()
				{
					TTL = TimeSpan.FromSeconds(_consulServiceInstance.TTL),
					DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(_consulServiceInstance.CriticalServiceUnregisterTime),
					Status = HealthStatus.Passing,
					ServiceID = _consulServiceInstance.Id,
				}
			};

			try
			{
				var result = await _client.Agent.ServiceRegister(agent);
				if (result.StatusCode != HttpStatusCode.OK)
					throw new Exception();

				_consulServiceInstance.IsRegistered = true;
				await SendHeartbeat();
			}
            catch
            {
				//TODO: Logging
				await Register(Math.Min(retryDelayMs * 2, _maxRetryDelay));
			}
		}
		private async Task<bool> IsServiceRegistered()
		{
			try
			{
				var res = await _client.Health.Service(
					_consulServiceInstance.ServiceName, 
					$"serviceid={_consulServiceInstance.Id} time={DateTime.Now}", true);
				if (res == null || res.Response == null || res.StatusCode != HttpStatusCode.OK)
					throw new Exception();

				foreach(var serviceEntry in res.Response)
                {
					var discoveredService = new DiscoveredService(serviceEntry);
					if (discoveredService != null
						&& discoveredService.Version.ToString() == _consulServiceInstance.Version)
						return true;
                }

				return false;
			}
            catch
            {
				_logger.LogWarning("There was a problem accessing Consul.");
				return false;
            }
		}


		public async Task<string> DiscoverService(DiscoverOptions options)
		{
			options.CompleteDiscoverOptions();

			var res = await _client.Health.Service(options.SearchServiceKey, "", true);
			if(res == null || res.StatusCode != HttpStatusCode.OK)
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
        public Task<List<string>> DiscoverServices(DiscoverOptions options)
        {
            throw new NotImplementedException();
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

			try
			{
				var result = await _client.Agent.ServiceDeregister(_consulServiceInstance?.Id);
				if (result.StatusCode == HttpStatusCode.OK)
				{
					status = ExecutionStatus.Good();
					status.Message = $"Service with id {_consulServiceInstance.Id} was successfully unregistred.";
				}
				else
				{
					status = ExecutionStatus.Bad();
					status.Message = $"There were some problems unregistering service with id {_consulServiceInstance.Id}.";
				}

			}
			catch
            {
				status = ExecutionStatus.Bad();
				status.Message = $"There were some problems unregistering service.";
            }

			return status;
		}


    }
}
