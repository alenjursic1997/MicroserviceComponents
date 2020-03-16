using ConfigCore.common;
using ConfigCore.common.models;
using ConfigCore.config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore
{
	public static class ServiceConfig
	{
		private static IConfig _config;

		public static IServiceCollection AddKumuluzConfig(this IServiceCollection services, Action<ConfigOptions> options)
		{
			ConfigOptions opt = new ConfigOptions();
			options?.Invoke(opt);
			_config = new Config(opt);
			ConfigProvider._config = _config;
			services.AddSingleton(_config);

			return services;
		}
	}
}
