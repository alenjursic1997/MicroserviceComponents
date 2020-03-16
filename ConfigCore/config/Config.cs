using ConfigCore.common;
using ConfigCore.common.converters;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.consul;
using ConfigCore.environment;
using ConfigCore.etcd;
using ConfigCore.file;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConfigCore.config
{
	public class Config : IConfig
	{
		private List<ConfigSource> _configSources;
		private readonly IConverter _converter;
		private readonly ILogger _logger;

		public Config(ConfigOptions options)
		{
			_logger = options.Logger ?? new NullLogger<Config>();

			_converter = new Converter(_logger);

			_configSources = new List<ConfigSource>();

			_configSources.Add(new EnvConfig(_logger));

			FileConfig fileConfig = new FileConfig(_logger, options.ConfigFilePath);
			if (fileConfig != null)
			{
				_configSources.Add(fileConfig);
			}
			else
			{
				_logger.LogInformation("Default path 'config.yaml' is used for configuration file.");
			}

			//Sort current list of config sources by their priority;
			SortConfigSources();

			//add extension config sources
			if (options.Extensions == null || options.Extensions == "")
				return;
			var configExtensions = options.Extensions.Split(',');
			foreach (string extension in configExtensions)
			{
				switch (extension.ToLower())
				{
					case "consul":
						ConsulConfig consulConfig = new ConsulConfig(this, _logger, _converter);
						_configSources.Add(consulConfig);
						break;
					case "etcd":
						EtcdConfig etcdConfig = new EtcdConfig(this, _logger, _converter);
						_configSources.Add(etcdConfig);
						break;
					default:
						_logger.LogWarning($"There is no implementation for '{extension}' extension.");
						break;
				}
			}

			//Sort current list of config sources by their priority;
			SortConfigSources();
		}

		public List<ConfigSource> GetConfigSources()
		{
			return _configSources;
		}

		private string GetValue(string key)
		{
			string value = null;

			foreach(ConfigSource configSource in _configSources)
			{
				value = configSource.GetValue(key);
				if (value != null)
					break;
			}

			return value;
		}

		public T Get<T>(string key)
		{
			var value = GetValue(key);
			return _converter.ConvertTo<T>(value);
		}

		public void Subscribe<T>(string key, Action<T> callback)
		{
			foreach(ConfigSource cf in _configSources)
			{
				cf.Subscribe(key, callback);
			}
		}


		private void SortConfigSources()
		{
			_configSources = _configSources.OrderBy(s => s.GetPriority()).ToList();
		}

	}
}
