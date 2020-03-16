using System;
using System.Collections.Generic;
using System.Text;
using HealthCore.Models;
using StackExchange.Redis;

namespace HealthCore.Checks
{
	public class RedisHealthCheck : HealthCheck
	{
		private readonly string _configuration;
		private HealthCheckResponse response = new HealthCheckResponse();

		public RedisHealthCheck(string configuration)
		{
			_configuration = configuration;
		}

		public override HealthCheckResponse CheckResponse()
		{
			try
			{
				var options = ConfigurationOptions.Parse(_configuration);
				var conn = ConnectionMultiplexer.Connect(options);

				while (conn != null && conn.IsConnecting) { };
				if (conn.IsConnected)
				{
					response.Up();
					return response;
				}
				else
				{
					response.Down();
					return response;
				}
			}
			catch
			{
				response.Down();
				return response;
			}
		}
	}
}
