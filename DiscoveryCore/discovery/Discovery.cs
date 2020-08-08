using ConfigCore.common.models;
using DiscoveryCore.common.interfaces;
using DiscoveryCore.common.models;
using DiscoveryCore.consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
			_configOptions.SetExtensions(options.Extension);
			_configOptions.SetLogger(_logger);

			switch(options.Extension)
			{
				case Extension.Consul: _discoverySource = new ConsulDiscovery(_configOptions, _logger);
					break;
				//case Extension.Etcd: _discoverySource = new EtcdDiscovery(_configOptions, _logger);
				//	break;
				default:
					break;
			}
		}

		public string RegisterService(RegisterOptions options)
		{
			return _discoverySource.RegisterService(options);
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
