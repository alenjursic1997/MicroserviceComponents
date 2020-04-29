namespace ConfigCore.config
{
	public static class ConfigProvider
	{
		internal static IConfig _config;

		//return configuration
		public static IConfig GetConfig()
		{
			return _config;
		}
	}
}
