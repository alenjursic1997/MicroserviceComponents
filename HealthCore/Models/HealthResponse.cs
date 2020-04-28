using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Models
{
	public class HealthResponse
	{
		public HealthResponse()
		{
			Checks = new List<HealthCheckResponse>();
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public State Status { get; set; }

		public IEnumerable<HealthCheckResponse> Checks { get; set; }



	}
}
