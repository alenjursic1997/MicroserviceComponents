using ConfigCore.common.models;
using ConfigCore.config;

namespace ConfigCore.common
{
	public static class Common
	{
		/// <summary>
		/// Load service configuration values
		/// </summary>
		/// <param name="conf">Implementation of IConfig interface</param>
		/// <returns>Service configuration values</returns>
		public static ServiceConfigurationValues LoadServiceConfiguration(IConfig conf)
		{
			ServiceConfigurationValues toReturn = new ServiceConfigurationValues();

			toReturn.envName = conf.Get<string>("kumuluzee.env.name");
			toReturn.name = conf.Get<string>("kumuluzee.name");
			toReturn.version = conf.Get<string>("kumulutee.version");
			toReturn.startRetryDelay = conf.Get<int>("kumuluzee.config.start-retry-delay-ms");
			toReturn.maxRetryDelay = conf.Get<int>("kumuluzee.config.max-retry-delay-ms");

			if (string.IsNullOrWhiteSpace(toReturn.envName))
				toReturn.envName = "dev";

			if (string.IsNullOrWhiteSpace(toReturn.name))
				toReturn.name = "";

			if (string.IsNullOrWhiteSpace(toReturn.version))
				toReturn.version = "1.0.0";

			if (toReturn.startRetryDelay == 0)
				toReturn.startRetryDelay = 500;

			if (toReturn.maxRetryDelay == 0)
				toReturn.maxRetryDelay = 900000;

			return toReturn;
		}
	}
}
