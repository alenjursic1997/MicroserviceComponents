using DiscoveryCore.common.models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryCore.common.interfaces
{
	public interface DiscoverySource
	{
		string RegisterService(RegisterOptions options);

		Task<string> DiscoverService(DiscoverOptions options);

		Task<ExecutionStatus> UnregisterService();
	}
}
