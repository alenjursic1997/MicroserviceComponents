using Google.Protobuf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigCore.common.models
{
	public class ConfigOptions
	{
		internal string ConfigFilePath { get; private set; }
		internal Extension[] Extensions { get; private set; }
		internal ILogger Logger { get; set; }


		public void SetConfigFilePath(string path)
		{
			ConfigFilePath = path;
		}

		public void SetExtensions(params Extension[] extensions)
        {
			Extensions = extensions;
        }

		public void SetLogger(ILogger logger)
		{
			Logger = logger;
		}
	}
}