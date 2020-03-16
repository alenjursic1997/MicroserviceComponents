using HealthCore.Checks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Models
{
	public class HealthOptions
	{
		public Dictionary<string, HealthCheck> HealthChecks { get; private set; }
		internal ILogger Logger { get; private set; }

		public HealthOptions()
		{
			HealthChecks = new Dictionary<string, HealthCheck>();
		}

		public void RegisterHealthCheck(string name, HealthCheck healthCheck)
		{
			HealthChecks.Add(name, healthCheck);
		}

		public void SetLogger(ILogger logger)
		{
			Logger = logger;
		}
	}
}
