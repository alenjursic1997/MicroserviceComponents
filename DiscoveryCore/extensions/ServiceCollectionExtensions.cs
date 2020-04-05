using DiscoveryCore.common.models;
using DiscoveryCore.discovery;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddKumuluzDiscovery(this IServiceCollection services, Action<InitializationOptions> options)
		{
			InitializationOptions opt = new InitializationOptions();
			options(opt);
			var discovery = new Discovery(opt);
			DiscoveryProvider._discovery = discovery;
			services.AddSingleton(discovery);

			return services;
		}
	}
}
