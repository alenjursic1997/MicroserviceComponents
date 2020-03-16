using DiscoveryCore.common.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.discovery
{
	public interface IDiscovery
	{
		string RegisterService(RegisterOptions options);

		string RegisterService();

		string UnregisterService();

		string DiscoverService(DiscoverOptions options);
	}
}
