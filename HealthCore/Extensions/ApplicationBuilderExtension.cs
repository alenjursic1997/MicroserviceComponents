using HealthCore.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Extensions
{
	public static class ApplicationBuilderExtension
	{
		public static IApplicationBuilder UseKumuluzHealth(this IApplicationBuilder app, string route = "/health")
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));


			if (route == null)
			{
				throw new ArgumentNullException(nameof(route), "Route must not be null!");
			}
			else if (route.Length == 0)
			{
				throw new ArgumentException(nameof(route), "Route must not be empty!");
			}
			else if (route == "/")
			{
				throw new ArgumentException(nameof(route), "Route must not be equals '/'!");
			}


			if (route[0] != '/')
			{
				route = route.Insert(0, "/");
			}
			if (route[route.Length - 1] == '/')
			{
				route = route.Remove(route.Length - 1);
			}

			Func<HttpContext, bool> predicate = c =>
			{
				List<string> validPaths = new List<string>
				{
					route,
					route + "/live",
					route + "/ready"
					//route + "/UI"
				};

				if (c.Request.Path.HasValue == false)
					return false;

				return c.Request.Path.HasValue &&
						validPaths.Contains(c.Request.Path.Value);
			};

			app.MapWhen(predicate, a => a.UseMiddleware<HealthMiddleware>());

			return app;
		}
	}
}
