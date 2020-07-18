using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.models
{
	public class ConfigOptions
	{
		internal string ConfigFilePath { get; private set; }
		internal string Extensions { get; private set; }
		internal int LogLevel { get; private set; }
		internal ILogger Logger { get; set; }


		public void SetConfigFilePath(string path)
		{
			ConfigFilePath = path;
		}

		public void SetExtension(string extensions)
		{
			Extensions = extensions;
		}

		public void SetLogLevel(int logLevel)
		{
			LogLevel = logLevel;
		}

		public void SetLogger(ILogger logger)
		{
			Logger = logger;
		}
	}
}