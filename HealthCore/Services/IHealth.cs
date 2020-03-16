using HealthCore.Checks;
using HealthCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Services
{
	public interface IHealth
	{
		/// <summary>
		/// Adds health check to registry.
		/// </summary>
		/// <param name="healthCheckName">Name of health check</param>
		/// <param name="healthCheck">Instance of health check implementation</param>
		void Register(string healthCheckName, HealthCheck healthCheck);

		/// <summary>
		/// Removes health check from registry.
		/// </summary>
		/// <param name="healthCheckName">Health check name</param>
		void Unregister(string healthCheckName);

		/// <summary>
		/// Executes health checks and returns results.
		/// </summary>
		/// <returns>List of responses</returns>
		IEnumerable<HealthCheckResponse> GetResults(HealthType type);

		bool IsEnabled();

		void Enable(bool enable);
	}
}
