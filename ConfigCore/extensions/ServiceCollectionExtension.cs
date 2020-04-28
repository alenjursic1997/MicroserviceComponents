using ConfigCore.common.models;
using ConfigCore.config;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigCore.extensions
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddKumuluzConfig(this IServiceCollection services, Action<ConfigOptions> options)
		{
			ConfigOptions opt = new ConfigOptions();
			options?.Invoke(opt);
			IConfig config = new Config(opt);
			ConfigProvider._config = config;
			services.AddSingleton(config);

			return services;
		}
	}
}
