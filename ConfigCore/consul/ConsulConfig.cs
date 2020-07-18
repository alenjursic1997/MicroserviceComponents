using ConfigCore.common;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.config;
using Consul;
using Microsoft.Extensions.Logging;
using System;
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

		public readonly ILogger _logger;
		public readonly IConfig _config;
		public readonly IConverter _converter;

		public IConsulClient client;
		public int startRetryDelay;
		public int maxRetryDelay;
		public string nametag;

		public ConsulConfig(IConfig config, ILogger logger, IConverter converter)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_converter = converter ?? throw new ArgumentNullException(nameof(converter));

			//getting source address
			string sourceAddress = _config.Get<string>("kumuluzee.config.consul.hosts");
			if (!string.IsNullOrWhiteSpace(sourceAddress))
			{
				sourceAddress = ADDRESS;
				client = new ConsulClient(c =>
				{
					c.Address = new Uri(sourceAddress);
				});
			}
			else
				client = null;


			//if client is null, return 
			if (client == null)
			{
				_logger.LogWarning("Thare was problem creating consul client.");
				return;
			}

			//get service configuration values from configuration sources
			ServiceConfigurationValues scv = Common.LoadServiceConfiguration(_config);
			startRetryDelay = scv.startRetryDelay;
			maxRetryDelay = scv.maxRetryDelay;
			nametag = $"environments/{ scv.envName}/services/{scv.name}/{scv.version}/config";


			var resultNametag = _config.Get<string>("kumuluzee.config.namespace");
			if (resultNametag != null)
				nametag = resultNametag;

			if (!string.IsNullOrWhiteSpace(nametag))
				nametag = nametag + "/";
		}


		//try to get value from consul configuration source
		public string GetValue(string key)
		{
			IKVEndpoint kv = this.client?.KV;

			try 
			{
				key = key.Replace('.', '/');

				var value = kv.Get(nametag + key)?.Result?.Response?.Value;
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

		public async void Watch<T>(string key, Action<T> callback, string oldValue = "", int retryDelay = 0)
		{
			//stop watching changes if callback is null
			if (callback == null)
				return;

			var cb = new Action<object>(o => callback((T)o)); //When using converter...is this even needed?

			key = key.Replace('.', '/');
			
			//trying to get response from consul configuration
			QueryResult<KVPair> response;
			try
			{
				await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
				response = await client.KV.Get(nametag + key);
			}
			catch
			{
				response = null;
			}

			if (response == null || response.StatusCode != HttpStatusCode.OK) {
				if (retryDelay == 0)
					retryDelay = startRetryDelay;
				Watch(key, callback, oldValue, Math.Min(2 * retryDelay, maxRetryDelay));
				return;
			}

			var kvPair = response.Response;
			if (kvPair?.Value != null)
			{
				var value = Encoding.UTF8.GetString(kvPair.Value, 0, kvPair.Value.Length);
				if (value != oldValue)
					cb(_converter.ConvertTo<T>(value));
				Watch(key, callback, value, startRetryDelay);
				return;
			}

			Watch(key, callback, oldValue, startRetryDelay);
			return;
		}
	}
}
