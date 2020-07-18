using ConfigCore.common;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.config;
using dotnet_etcd;
using Etcdserverpb;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConfigCore.etcd
{
	public class EtcdConfig : ConfigSource
	{
		private const string NAME = "etcd";
		private const int PRIORITY = 150;
		private const string ADDRESS = "http://localhost:2379";

		public readonly ILogger _logger;
		public readonly IConfig _config;
		public readonly IConverter _converter;

		public EtcdClient client;
		public int startRetryDelay;
		public int maxRetryDelay;
		public string nametag;

		public EtcdConfig(IConfig config, ILogger logger, IConverter converter)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_converter = converter ?? throw new ArgumentNullException(nameof(converter));

			//getting source address
			string sourceAddress = _config.Get<string>("kumuluzee.config.etcd.hosts");
			if (!string.IsNullOrEmpty(sourceAddress))
				sourceAddress = ADDRESS;


			string username = _config.Get<string>("kumuluzee.config.etcd.username");
			if (!string.IsNullOrEmpty(username))
			{
				var password = _config.Get<string>("kumuluzee.config.etcd.password") ?? "";
				var cert = _config.Get<string>("kumuluzee.config.etcd.cert") ?? "";
				client = new EtcdClient(sourceAddress,
							username: username,
							password: password,
							caCert: cert);
			}
			else
				client = new EtcdClient(sourceAddress);

			if (client == null)
            {
				_logger.LogWarning("Thare was problem creating etcd client.");
				return;
			}

			ServiceConfigurationValues scv = Common.LoadServiceConfiguration(_config);
			startRetryDelay = scv.startRetryDelay;
			maxRetryDelay = scv.maxRetryDelay;
			nametag = $"environments/{scv.envName}/services/{scv.name}/{scv.version}/config";

			//overwrite nametag whenever it exists in configuration file
			var resultNametag = _config.Get<string>("kumuluzee.config.namespace");
			if (resultNametag != null)
				nametag = resultNametag;

			if(!string.IsNullOrWhiteSpace(nametag))
				nametag = nametag + "/";
		}

	
		public string GetValue(string key)
		{
			try
			{
				key = key.Replace('.', '/');
				var value = client?.GetValAsync(nametag + key)?.Result;
				return value;
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
			if (callback == null)
				return;

			var cb = new Action<object>(o => callback((T)o));

			key = key.Replace('.', '/');
			RangeResponse response;
			try
			{
				await Task.Delay(TimeSpan.FromMilliseconds(retryDelay));
				response = await client?.GetAsync(nametag + key);
			}
			catch 
			{
				response = null;
			}

			if(response == null)
			{
				if (retryDelay == 0)
					retryDelay = startRetryDelay;
				Watch(key, callback, oldValue, Math.Min(2 * retryDelay, maxRetryDelay));
				return;
			}

			if(response.Count > 0)
			{
				var value = response.Kvs[0].Value.ToStringUtf8();
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
