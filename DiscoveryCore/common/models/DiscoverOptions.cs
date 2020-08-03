using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class DiscoverOptions
	{
		public string ServiceName { get; set; }
		public string Environment { get; set; }
		public string Version { get; set; }
		public AccessType AccessType { get; set; }
		internal string AccessTypeValue
		{
			get
			{
				if (AccessType == AccessType.Direct)
					return "direct";
				else
					return "gateway";
			}
		}

		public string SearchServiceKey { get { return $"{Environment}-{ServiceName}"; } }
	}

	public enum AccessType { 
		Direct,
		Gateway
	}
}
