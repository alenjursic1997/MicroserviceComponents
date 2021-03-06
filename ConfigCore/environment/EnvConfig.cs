﻿using ConfigCore.common.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;


namespace ConfigCore.environment
{
	public class EnvConfig : ConfigSource
	{
		private const string NAME = "evnironment";
		private const int PRIORITY = 0;
		private readonly ILogger _logger;

		public EnvConfig(ILogger logger)
		{
			//set logger
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public string GetValue(string key)
		{
			string value = null;

			_logger.LogInformation($"Searching value for key '{key}' in system variables.");

			//loop trought all environment targets
			foreach (EnvironmentVariableTarget t in (EnvironmentVariableTarget[])Enum.GetValues(typeof(EnvironmentVariableTarget)))
			{
				//getting value from evironment target with name equals key value
				string tempKey = key;
				value = Environment.GetEnvironmentVariable(key, t);
				//if value doesn't exist replace - with _
				if (value == null)
				{
					Regex rgx = new Regex($"[^a-zA-Z0-9 -]");
					tempKey = rgx.Replace(tempKey, "_");
					value = Environment.GetEnvironmentVariable(tempKey, t);
				}
				//if value doesn't exist replace change key value to upper case letters
				if (value == null)
				{
					tempKey = tempKey.ToUpper();
					value = Environment.GetEnvironmentVariable(tempKey, t);
				}
				//if value isn't null, get out from loop
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
