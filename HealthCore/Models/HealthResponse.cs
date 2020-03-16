using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCore.Models
{
	public class HealthResponse
	{
		private State _outcome;

		private IEnumerable<HealthCheckResponse> _checks; 

		public HealthResponse()
		{
			_checks = new List<HealthCheckResponse>();
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public State Status 
		{
			get
			{
				return _outcome;
			}
			set
			{
				_outcome = value;
			}
		}

		public IEnumerable<HealthCheckResponse> Checks 
		{
			get
			{
				return _checks;
			}
			set
			{
				_checks = value;
			}
		}



	}
}
