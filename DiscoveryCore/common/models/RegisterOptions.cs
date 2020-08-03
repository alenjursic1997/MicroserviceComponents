using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class RegisterOptions
	{
		public string ServiceName { get; set; }
		public int TTL { get; set; }
		public int PingInterval { get; set; }
		public string Environment { get; set; }
		public string Version { get; set; }
		public bool Singleton { get; set; }
	}
}
