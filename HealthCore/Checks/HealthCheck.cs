using HealthCore.Models;

namespace HealthCore.Checks
{
	public abstract class HealthCheck
	{
		public abstract HealthCheckResponse CheckResponse();
	}
}
