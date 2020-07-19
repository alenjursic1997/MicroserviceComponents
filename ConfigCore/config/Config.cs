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
using System.Linq;

namespace ConfigCore.config
{
	public class Config : IConfig
	{
		private List<ConfigSource> _configSources;
		private readonly IConverter _converter;
		private readonly ILogger _logger;

		public Config(ConfigOptions options)
		{
			//if logger is null, create new logger
			_logger = options.Logger ?? new NullLogger<Config>();

			//create new converter
			_converter = new Converter(_logger);

			_configSources = new List<ConfigSource>();

			//create and add new evironment configuration
			_configSources.Add(new EnvConfig(_logger));

			//create and add new file configuration
			FileConfig fileConfig = new FileConfig(_logger, options.ConfigFilePath);
			if (fileConfig != null)
				_configSources.Add(fileConfig);
			else
				_logger.LogInformation("Default path 'config.yaml' is used for configuration file.");

			//sort current list of config sources by their priority;
			SortConfigSources();

			foreach (var extension in options.Extensions)
			{
				switch (extension)
				{
					case Extension.Consul:
						//create and add new consul configuration
						ConsulConfig consulConfig = new ConsulConfig(this, _logger, _converter);
						_configSources.Add(consulConfig);
						break;
					case Extension.Etcd:
						//create and add new etcd configuration
						EtcdConfig etcdConfig = new EtcdConfig(this, _logger, _converter);
						_configSources.Add(etcdConfig);
						break;
					default:
						_logger.LogWarning($"There is no implementation for '{extension}' extension.");
						break;
				}
			}

			//sort current list of config sources by their priority;
			SortConfigSources();
		}

		//get all configuration sources
		public List<ConfigSource> GetConfigSources()
		{
			return _configSources;
		}

		//get value for given key from all configuration source with lower priority number
		//value is returned as string
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

		//return value for given key as type of T 
		public T Get<T>(string key)
		{
			//get value from configuration sources
			var value = GetValue(key);
			//convert string value to type of T
			return _converter.ConvertTo<T>(value);
		}

		//subscribe action of type T to on value changes for given key
		public void Subscribe<T>(string key, Action<T> callback)
		{
			foreach(ConfigSource cf in _configSources)
			{
				cf.Subscribe(key, callback);
			}
		}


		//sort all configuration sources by their priority number (from min. to max.)
		private void SortConfigSources()
		{
			_configSources = _configSources.OrderBy(s => s.GetPriority()).ToList();
		}

	}
}
