using HealthCore.Models;
using HealthCore.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCore.Controllers
{
	public class HealthMiddleware
	{ 

        private readonly IHealth _healthCheckRegistry;

		public HealthMiddleware(RequestDelegate next, IHealth healthCheckRegistry)
		{
			_healthCheckRegistry = healthCheckRegistry;
		}


        public async Task Invoke(HttpContext context)
        {
            var paths = context.Request.Path.Value.Split('/');

			HealthResponse healthResponse;
			if (paths[paths.Length - 1] == "live")
				healthResponse = GetHealthByType(HealthType.Liveness);
			else if(paths[paths.Length - 1] == "ready")
				healthResponse = GetHealthByType(HealthType.Readiness);
			else
				healthResponse = GetHealthByType(HealthType.Any);

			
			if (healthResponse == null || healthResponse.Status == State.DOWN)
				context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
			else
				context.Response.StatusCode = StatusCodes.Status200OK;

			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync(JsonConvert.SerializeObject(healthResponse));
        }

		private HealthResponse GetHealthByType(HealthType type)
		{
			try
			{
				// get results
				var results = _healthCheckRegistry.GetResults(type);

				// prepare response
				HealthResponse healthResponse = new HealthResponse();
				healthResponse.Checks = results;
				healthResponse.Status = State.UP;

				// check if any check is down
				if (results.Where(e => State.DOWN.Equals(e.Status)).Any())
					healthResponse.Status = State.DOWN;

				return healthResponse;
			}
			catch
			{
				return null;
			}
		}

	}
}
