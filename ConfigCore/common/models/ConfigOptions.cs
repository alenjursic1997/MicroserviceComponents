using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// V diplomski nalogi se ta objekt uporablja pri kreiranju novega Utila in Bundla
/// </summary>

namespace ConfigCore.common.models
{
	public class ConfigOptions
	{
		public string ConfigFilePath { get; private set; }
		public string Extensions { get; private set; }
		public string ExtensionNamespace { get; private set; }
		public int LogLevel { get; private set; }
		public ILogger Logger { get; set; }


		public void SetConfigFilePath(string path)
		{
			ConfigFilePath = path;
		}

		public void SetExtension(string extensions)
		{
			Extensions = extensions;
		}

		public void SetExtensionNamespace(string extensionNamespace)
		{
			ExtensionNamespace = extensionNamespace;
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