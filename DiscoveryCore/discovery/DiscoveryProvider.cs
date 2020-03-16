using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.discovery
{
	public static class DiscoveryProvider
	{
		internal static IDiscovery _discovery;

		public static IDiscovery GetDiscovery()
		{
			return _discovery;
		}
	}
}
