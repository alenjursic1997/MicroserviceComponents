using DiscoveryCore.common.models;
using DiscoveryCore.discovery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore
{
	public static class ServiceConfig
	{
		private static IDiscovery _discovery;

		public static IServiceCollection AddKumuluzDiscovery(this IServiceCollection services, Action<InitializationOptions> options)
		{
			InitializationOptions opt = new InitializationOptions();
			options(opt);
			_discovery = new Discovery(opt);
			DiscoveryProvider._discovery = _discovery;
			services.AddSingleton(_discovery);

			return services;
		}

	}
}
