using HealthCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Checks
{
	public abstract class HealthCheck
	{
		public abstract HealthCheckResponse CheckResponse();
	}
}
