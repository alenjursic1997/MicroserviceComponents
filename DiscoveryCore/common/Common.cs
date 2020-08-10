using ConfigCore.config;
using DiscoveryCore.common.models;
using SemVer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryCore.common
{
	public static class Common
	{
		public static (int startRetryDelay, int maxRetryDelay) GetRetryDelays(IConfig config)
		{
			int srd = 0, mrd = 0;

			if (config != null)
			{
				srd = config.Get<int?>("kumuluzee.config.start-retry-delay-ms") ?? 500;
				mrd = config.Get<int?>("kumuluzee.config.max-retry-delay-ms") ?? 900000;
			}

			return (srd, mrd);
		}

		public static RegisterConfiguration GetServiceRegisterConfiguration(IConfig config, RegisterOptions options)
		{
			RegisterConfiguration registerConfig = new RegisterConfiguration();

			registerConfig.Name = config?.Get<string>("kumuluzee.name");
			if (string.IsNullOrWhiteSpace(registerConfig.Name))
				registerConfig.Name = "service";
			registerConfig.EnvName = config?.Get<string>("kumuluzee.env.name");
			if (string.IsNullOrWhiteSpace(registerConfig.EnvName))
				registerConfig.EnvName = "dev";
			registerConfig.Protocol = config?.Get<string>("kumuluzee.discovery.consul.protocol");
			if (string.IsNullOrWhiteSpace(registerConfig.Protocol))
				registerConfig.Protocol = "http";

			registerConfig.Singleton = options.Singleton;
			registerConfig.Discovery.PingInterval = config?.Get<int?>("kumuluzee.discovery.ping-interval") ?? 20;
			registerConfig.Discovery.TTL = config?.Get<int?>("kumuluzee.discovery.ttl") ?? 30;
			registerConfig.Server.Address = config?.Get<string>("kumuluzee.server.http.address");
			registerConfig.Server.BaseUrl = config?.Get<string>("kumuluzee.server.base-url");
			registerConfig.Server.Port = config?.Get<int?>("kumuluzee.server.http.port") ?? 5000;
			registerConfig.Version = config?.Get<string>("kumuluzee.version") ?? "1.0.0";
			registerConfig.CriticalServiceUnregisterTime = config?.Get<int?>("kumuluzee.discovery.consul.deregister-critical-service-after-s") ?? 60;

			if (!string.IsNullOrWhiteSpace(options.ServiceName))
				registerConfig.Name = options.ServiceName;
			if (options.PingInterval > 0)
				registerConfig.Discovery.PingInterval = options.PingInterval;
			if (options.TTL > 0)
				registerConfig.Discovery.TTL = options.TTL;
			if (!string.IsNullOrWhiteSpace(options.Version))
				registerConfig.Version = options.Version;
			if (!string.IsNullOrWhiteSpace(options.Environment))
				registerConfig.EnvName = options.Environment;

			return registerConfig;
		}


		public static void CompleteDiscoverOptions (this DiscoverOptions options)
		{
			if (options.ServiceName == null)
				options.ServiceName = "";
			if (string.IsNullOrWhiteSpace(options.Environment))
				options.Environment = "dev";
			if (string.IsNullOrWhiteSpace(options.Version))
				options.Version = ">=0.0.0";
			if (string.IsNullOrWhiteSpace(options.AccessTypeValue))
				options.AccessType = AccessType.Gateway;
		}

		public static string GetRandomServiceInstance(List<DiscoveredService> discoveredServices,
			List<GatewayURLWatch> gateways, DiscoverOptions options)
		{
			Range desiredVersionRange;
			try { desiredVersionRange = new Range(options.Version); }
			catch { desiredVersionRange = null; }

			if (desiredVersionRange == null)
				return null;

			var latestVersion = desiredVersionRange.MaxSatisfying(discoveredServices.Select(e => e.Version));
			var validServices = discoveredServices.Where(e => e.Version == latestVersion).ToList();

			if (validServices.Count == 0)
				return null;

			var randomService = validServices[new Random().Next(validServices.Count)];
			var watchNamespace = $"/environments/{options.Environment}/services/{options.ServiceName}/{randomService.Version.ToString()}";
			var serviceGatewayURL = gateways.Where(e => e.Id == watchNamespace).Select(e =>e.URL).FirstOrDefault();

			if (!string.IsNullOrEmpty(serviceGatewayURL) && options.AccessType == AccessType.Gateway)
				return serviceGatewayURL;
			else if (!string.IsNullOrEmpty(randomService.DirectURL))
				return randomService.DirectURL;
			else
				return null;
		}

	}
}