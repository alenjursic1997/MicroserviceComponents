using ConfigCore.common.models;
using ConfigCore.config;
using DiscoveryCore.common;
using DiscoveryCore.common.interfaces;
using DiscoveryCore.common.models;
using dotnet_etcd;
using Etcdserverpb;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryCore.etcd
{
	public class EtcdDiscovery : DiscoverySource
	{
		private const string ADDRESS = "http://localhost:2379";

		private ConfigOptions _configOptions;
		private readonly IConfig _config;
		private readonly ILogger _logger;

		private EtcdClient _client;
		public int _startRetryDelay;
		public int _maxRetryDelay;
		public string _protocol;

		private RegisterConfiguration _regConfig;
		private EtcdServiceInstance _etcdServiceInstance;

		public EtcdDiscovery(ConfigOptions options, ILogger logger)
		{
			_configOptions = options;
			_config = new Config(options);
			_logger = logger;

			var delays = Common.GetRetryDelays(_config);
			_startRetryDelay = delays.StartRetryDelay;
			_maxRetryDelay = delays.MaxRetryDelay;
			//logging

			//getting source address
			string sourceAddress = _config.Get<string>("kumuluzee.discovery.etcd.hosts");
			if (string.IsNullOrEmpty(sourceAddress))
				sourceAddress = ADDRESS;

			//creating consul client
			string username = _config.Get<string>("kumuluzee.discovery.etcd.username");
			if (!string.IsNullOrEmpty(username))
			{
				var password = _config.Get<string>("kumuluzee.discovery.etcd.password") ?? "";
				var cert = _config.Get<string>("kumuluzee.discovery.etcd.password") ?? "";
				_client = new EtcdClient(sourceAddress,
							username: username,
							password: password,
							caCert: cert);
			}
			else
			{
				_client = new EtcdClient(sourceAddress);
			}

			if (_client == null)
			{
				_logger.LogWarning("Thare was problem creating etcd client.");
			}

			_protocol = _config.Get<string>("kumuluzee.discovery.etcd.protocol");
			if (string.IsNullOrEmpty(_protocol))
				_protocol = "http";

		}

		public string RegisterService(RegisterOptions options)
		{
			_regConfig = Common.GetServiceRegisterConfiguration(_config, options); //TODO: what is the point of this method???

			//? TO BE DELETED!!!
			_regConfig.Name = "Ime";
			_regConfig.EnvName = "Razvoj";
			_regConfig.Discovery.TTL = 30;
			_regConfig.Discovery.PingInterval = 20;
			_regConfig.Server.BaseUrl = "http://localhost:5000";
			_regConfig.Server.Port = 5000;
			_regConfig.Server.Address = "Address";
			_regConfig.Version = "1.2.3";

			_etcdServiceInstance = new EtcdServiceInstance(_regConfig, isSingleton: options.Singleton);
			Run(_startRetryDelay);
			return _etcdServiceInstance.Id.ToString();
		}
		private async void Run(int retryDelayMs)
		{
			ExecutionStatus status;
			bool firstRunStatus = false;

			if (_etcdServiceInstance.IsRegistered)
			{
				status = await TTLUpdate();
				if (!status.Successful)
				{
					_etcdServiceInstance.IsRegistered = false;
				}
			}
			else
			{
				status = await Register();
				if (status.Successful)
				{
					firstRunStatus = true;
					_etcdServiceInstance.IsRegistered = true;
				}
			}

			if (status.Successful)
			{
				if (!firstRunStatus)
				{
					await Task.Delay(_regConfig.Discovery.PingInterval); //TODO: check if correct
					firstRunStatus = false;
				}
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
		private async Task<ExecutionStatus> Register()
		{
			if (await IsServiceRegistred() && _etcdServiceInstance.IsSingleton)
			{
				return ExecutionStatus.Bad();
			}

			MemberAddRequest request = new MemberAddRequest();
			request.PeerURLs.Add("http://localhost:5000");

			MemberAddResponse res = await _client.MemberAddAsync(request);


			if (res.Header.MemberId <= 0)
			{
				return ExecutionStatus.Bad();
			}
			_etcdServiceInstance.Id = res.Header.MemberId;
			return ExecutionStatus.Good();
		}
		private async Task<bool> IsServiceRegistred()
		{
			return false; //TODO: check if length of serviceEntries is more than 0
		}
		private async Task<ExecutionStatus> TTLUpdate()
		{
			return ExecutionStatus.Good();
		}

		public string DiscoverService(DiscoverOptions options)
		{
			throw new NotImplementedException();
		}


		public async Task<ExecutionStatus> UnregisterService()
		{
			throw new NotImplementedException();
		}

		Task<string> DiscoverySource.DiscoverService(DiscoverOptions options)
		{
			throw new NotImplementedException();
		}
	}

	internal class EtcdServiceInstance
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string VersionTag { get; set; }
		public bool IsSingleton { get; set; }
		public bool IsRegistered { get; set; }

		public EtcdServiceInstance()
		{

		}

		public EtcdServiceInstance(RegisterConfiguration regConfig, bool isSingleton = true)
		{
			Id = 0;
			Name = $"{regConfig.EnvName}-{regConfig.Name}";
			VersionTag = $"version={regConfig.Version}";
			IsSingleton = isSingleton;
		}
	}
}
