using ConfigCore.config;
using ConfigCore.common.models;
using DiscoveryCore.common;
using DiscoveryCore.common.models;
using System;
using System.Collections.Generic;
using System.Text;
using DiscoveryCore.common.interfaces;
using DiscoveryCore.consul;
using DiscoveryCore.etcd;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;

namespace DiscoveryCore.discovery
{
	public class Discovery : IDiscovery
	{
		private readonly ConfigOptions _configOptions;
		private DiscoverySource _discoverySource;
		private ILogger _logger;

		public Discovery(InitializationOptions options)
		{
			_logger = options.Logger ?? new NullLogger<Discovery>();

			_configOptions = new ConfigOptions();
			_configOptions.SetConfigFilePath(options.ConfigFilePath);
			_configOptions.SetExtension(options.Extension);
			_configOptions.SetLogger(_logger);

			switch(options.Extension.ToLower())
			{
				case "consul": _discoverySource = new ConsulDiscovery(_configOptions, _logger);
					break;
				case "etcd": _discoverySource = new EtcdDiscovery(_configOptions, _logger);
					break;
				default:
					break;
			}
		}

		public string RegisterService(RegisterOptions options)
		{
			_discoverySource.RegisterService(options);

			return "ServiceID";
		}

		public string RegisterService()
		{
			return RegisterService(new RegisterOptions());
		}

		public string UnregisterService()
		{
			var status = _discoverySource?.UnregisterService().Result;
			return status?.Message ?? "No service registered";
		}

		public string DiscoverService(DiscoverOptions options)
		{
			return _discoverySource.DiscoverService(options).Result;
		}
	}
}
