using HealthCore.Attributes;
using HealthCore.Checks;
using HealthCore.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HealthCore.Services
{
	public class Health : IHealth
	{
		private static Dictionary<string, HealthCheck> _healthChecks;
		private readonly ILogger _logger;

		public Health(HealthOptions healthOptions)
		{
			if (healthOptions == null)
				_healthChecks = new Dictionary<string, HealthCheck>();
			else
				_healthChecks = healthOptions.HealthChecks;

			_logger = healthOptions.Logger ?? new NullLogger<Health>();
		}

		public void Register(string healthCheckName, HealthCheck healthCheck)
		{
			_healthChecks.Add(healthCheckName, healthCheck);

			_logger.LogInformation($"Health check with name '{healthCheckName}' was successfully registered.");
		}

		public void Unregister(string healthCheckName)
		{
			if (!_healthChecks.ContainsKey(healthCheckName))
			{
				_logger.LogWarning($"Health check with name '{healthCheckName}' was not registered.");
				return;
			}

			_healthChecks.Remove(healthCheckName);

			_logger.LogInformation($"Health check with name '{healthCheckName}' was successfully unregistered.");
		}

		public IEnumerable<HealthCheckResponse> GetResults(HealthType type)
		{
			_logger.LogInformation("Getting health check results");

			foreach (var pair in _healthChecks)
			{
				if (!IsValidType(pair.Value, type))
					continue;

				var response = pair.Value?.CheckResponse();
				if (response == null) continue;

				response.Name = pair.Key;
				yield return response;
			}
		}

		private bool IsValidType(HealthCheck check, HealthType type)
		{
			Type attributeType = null;
			if(type == HealthType.Liveness)
			{
				attributeType = typeof(LivenessAttribute);
			}
			else if(type == HealthType.Readiness)
			{
				attributeType = typeof(ReadinessAttribute);
			}

			if (attributeType == null)
				return true;

			return check.GetType().GetCustomAttributes(attributeType, true).Any();
		}
	}
}
