using ConfigCore.common;
using ConfigCore.common.converters;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.config;
using Consul;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConfigCore.consul
{
	public class ConsulConfig : ConfigSource
	{
		private const string NAME = "consul";
		private const int PRIORITY = 100;
		private const string ADDRESS = "http://localhost:8500";

		public ConsulClient client;
		public int startRetryDelay;
		public int maxRetryDelay;
		public string nametag;
		public readonly ILogger _logger;
		public readonly IConfig _config;
		public readonly IConverter _converter;

		public ConsulConfig(IConfig conf, ILogger logger, IConverter converter)
		{
			_logger = logger;
			_config = conf;
			_converter = converter;

			//getting source address
			string sourceAddress = conf.Get<string>("kumuluzee.config.consul.hosts");
			if(!string.IsNullOrEmpty(sourceAddress))
				sourceAddress = ADDRESS;


			//creating consul client
			if (sourceAddress.Length == 0)
			{
				client = null;
			}

			client = new ConsulClient(c =>
			{
				c.Address = new Uri(sourceAddress);
			});



			if (client == null)
			{
				_logger.LogWarning("Thare was problem creating consul client.");
			}

			ServiceConfigurationValues scv = Common.loadServiceConfiguration(conf);
			startRetryDelay = scv.startRetryDelay;
			maxRetryDelay = scv.maxRetryDelay;
			nametag = "environments/" + scv.envName + "/services/" + scv.name + "/" + scv.version + "/config";

			var resultNametag = conf.Get<string>("kumuluzee.config.namespace");
			if (resultNametag != null)
				nametag = resultNametag;

			if (!string.IsNullOrEmpty(nametag))
				nametag = nametag + "/";
		}



		public string GetValue(string key)
		{
			IKVEndpoint kv = this.client?.KV;

			try
			{
				key = key.Replace('.', '/');

				var value = kv.Get(nametag + key)?.Result?.Response?.Value; //TODO: podati je treba pravi string
				if (value == null)
					return null;

				return Encoding.UTF8.GetString(value, 0, value.Length);
			}
			catch
			{
				_logger.LogInformation($"Unable to get value of key '{nametag + key}'");
				return null;
			}
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
			Watch(key, callback);
		}

		public async void Watch<T>(string key, Action<T> callback, string oldValue = "", int retryDelay = 0, ulong waitIndex = 0)
		{
			if (callback == null)
				return;
			var cb = new Action<object>(o => callback((T)o)); //When using converter...is this even needed?

			key = key.Replace('.', '/');
			var options = new QueryOptions() { WaitIndex = 0, WaitTime = TimeSpan.FromMinutes(10) };

			QueryResult<KVPair> response;
			try
			{
				response = await client.KV.Get(key, options);
			}
			catch
			{
				response = null;
			}

			if (response == null || response.StatusCode != HttpStatusCode.OK) { 
				await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
				Watch(key, callback, oldValue, Math.Max(2 * retryDelay, maxRetryDelay));
				return;
			}

			var kvPair = response.Response;
			if (kvPair?.Value != null)
			{
				var value = Encoding.UTF8.GetString(kvPair.Value, 0, kvPair.Value.Length);
				if (value != oldValue)
					cb(_converter.ConvertTo<T>(value));
				Watch(key, callback, value, startRetryDelay, response.LastIndex);
				return;
			}

			Watch(key, callback, oldValue, retryDelay: startRetryDelay, waitIndex: response.LastIndex);

			return;
		}


	}
}
