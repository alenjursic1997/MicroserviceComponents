using ConfigCore.common.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscoveryCore.common.models
{
	public class InitializationOptions
	{
		internal Extension Extension { get; private set; }

		internal string ConfigFilePath { get; private set; }

		internal ILogger Logger { get; private set; }


		public void SetExtension(DiscoveryExtension extension)
		{
			switch(extension)
            {
				case DiscoveryExtension.Consul:
					Extension = Extension.Consul;
					break;
            }
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

	public enum DiscoveryExtension
    {
		Consul
    }
}

