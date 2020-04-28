using HealthCore.Models;
using HealthCore.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Extensions
{
	public static class ServiceCollectionExtension
	{
		public static IServiceCollection AddKumuluzHealth(this IServiceCollection services, Action<HealthOptions> options = null)
		{
			HealthOptions opt = new HealthOptions();
			options?.Invoke(opt);
			IHealth healthRegistry = new Health(opt);
			services.AddSingleton(healthRegistry);
			return services;
		}
	}
}
