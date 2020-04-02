using ConfigCore.common.models;
using ConfigCore.config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigCore.common
{
	public static class Common
	{
		public static ServiceConfigurationValues loadServiceConfiguration(IConfig conf)
		{
			ServiceConfigurationValues toReturn = new ServiceConfigurationValues();

			toReturn.envName = conf.Get<string>("kumuluzee.env.name");
			toReturn.name = conf.Get<string>("kumuluzee.name");
			toReturn.version = conf.Get<string>("kumulutee.version");
			toReturn.startRetryDelay = conf.Get<int>("kumuluzee.config.start-retry-delay-ms");
			toReturn.maxRetryDelay = conf.Get<int>("kumuluzee.config.max-retry-delay-ms");

			if (toReturn.envName == null || toReturn.envName == "")
				toReturn.envName = "dev";

			if (toReturn.name == null)
				toReturn.name = "";

			if (toReturn.version == null || toReturn.version == "")
				toReturn.version = "1.0.0";

			if (toReturn.startRetryDelay == 0)
				toReturn.startRetryDelay = 500;

			if (toReturn.maxRetryDelay == 0)
				toReturn.maxRetryDelay = 900000;

			return toReturn;
		}
	}
}
