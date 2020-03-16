using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class RegisterConfiguration
	{
		public string Name { get; set; }
		public ServerParams Server { get; set; }
		public string EnvName { get; set; }
		public string Version { get; set; }
		public DiscoveryParams Discovery { get; set; }

		public RegisterConfiguration()
		{
			Server = new ServerParams();
			Discovery = new DiscoveryParams();
		}
	}

	public class ServerParams
	{
		public string BaseUrl { get; set; }
		public string Address { get; set; }
		public int Port { get; set; }
		
	}

	public class DiscoveryParams
	{
		public int TTL { get; set; }
		public int PingInterval { get; set; }
	}
}
