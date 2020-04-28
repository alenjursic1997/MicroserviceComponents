using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HealthCore.Models
{
	[JsonObject]
	public class HealthCheckResponse
	{
		public void Up()
		{
			Status = State.UP;
		}
		public void Down()
		{
			Status = State.DOWN;
		}

		public State Status { get; private set; }

		public string Name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object Data { get; set; }

	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum State { UP, DOWN }
}
