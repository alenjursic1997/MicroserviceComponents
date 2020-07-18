using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class InitializationOptions
	{
		internal string Extension { get; private set; }

		internal string ConfigFilePath { get; private set; }

		internal ILogger Logger { get; private set; }


		public void SetExtensions(string extension)
		{
			Extension = extension;
		}

		public void SetConfigFilePath(string filePath)
		{
			ConfigFilePath = filePath;
		}

		public void SetLogger(ILogger logger)
		{
			Logger = logger;
		}
	}
}

