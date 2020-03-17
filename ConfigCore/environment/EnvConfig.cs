using ConfigCore.common.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Zaenkrat deluje samo za uporabniške spremenljivke
/// </summary>

namespace ConfigCore.environment
{
	public class EnvConfig : ConfigSource
	{
		private const string NAME = "evnironment";
		private const int PRIORITY = 0;
		private readonly ILogger _logger;

		public EnvConfig(ILogger logger)
		{
			_logger = logger;
		}

		public string GetValue(string key)
		{
			string value = null;

			_logger.LogInformation($"Searching value for key '{key}' in system variables.");

			foreach (EnvironmentVariableTarget t in (EnvironmentVariableTarget[])Enum.GetValues(typeof(EnvironmentVariableTarget)))
			{
				string tempKey = key;
				value = Environment.GetEnvironmentVariable(key, t);
				if (value == null)
				{
					Regex rgx = new Regex($"[^a-zA-Z0-9 -]");
					tempKey = rgx.Replace(tempKey, "_");
					value = Environment.GetEnvironmentVariable(tempKey, t);
				}
				if (value == null)
				{
					tempKey = tempKey.ToUpper();
					value = Environment.GetEnvironmentVariable(tempKey, t);
				}
				if (value != null)
					break;
			}

			return value;
		}

		public string GetName()
		{
			return NAME;
		}

		public int GetPriority()
		{
			return PRIORITY;

		}

		public void Subscribe<T>(string key, Action<T> callback)
		{
			return;
		}
	}
}
