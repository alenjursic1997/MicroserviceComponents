using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.models
{
	public class ServiceConfigurationValues
	{
		public string envName { get; set; }
		public string name { get; set; }
		public string version { get; set; }
		public int startRetryDelay { get; set; }
		public int maxRetryDelay { get; set; }

		public ServiceConfigurationValues()
		{
			name = "";
			envName = "dev";
			version = "1.0.0";
			startRetryDelay = 500;
			maxRetryDelay = 900000;
		}
	}
}
