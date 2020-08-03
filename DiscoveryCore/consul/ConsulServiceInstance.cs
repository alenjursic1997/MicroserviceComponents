using DiscoveryCore.common.models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DiscoveryCore.consul
{
	internal class ConsulServiceInstance
	{
		public string Id { get; set; }
		public string ServiceName { get; set; }
		public string Environment { get; set; }
		public string Version { get; set; }
		public string VersionTag { get; set; }
		public string Protocol { get; set; }
		public int Port { get; set; }
		public string Address { get; set; }
		public string BaseUrl { get; set; }
		public int TTL { get; set; }
		public bool IsSingleton { get; set; }
		public int CriticalServiceUnregisterTime { get; set; }
		public bool IsRegistered { get; set; }

		public ConsulServiceInstance(RegisterConfiguration regConfig)
		{
			ServiceName = $"{regConfig.EnvName}-{regConfig.Name}";
			Environment = regConfig.EnvName;
			Id = $"{regConfig.Name}-{Guid.NewGuid()}";
			Version = regConfig.Version;
			VersionTag = $"version={regConfig.Version}";
			Protocol = regConfig.Protocol;
			Port = regConfig.Server.Port;
			Address = regConfig.Server.Address;
			BaseUrl = regConfig.Server.BaseUrl;
			TTL = regConfig.Discovery.TTL;
			CriticalServiceUnregisterTime = regConfig.CriticalServiceUnregisterTime;
			IsSingleton = regConfig.Singleton;
		}
	}
}
