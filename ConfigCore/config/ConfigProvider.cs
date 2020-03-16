using System;
using System.Collections.Generic;
using System.Text;
using ConfigCore.common;

namespace ConfigCore.config
{
	public static class ConfigProvider
	{
		internal static IConfig _config;

		public static IConfig GetConfig()
		{
			return _config;
		}
	}
}
