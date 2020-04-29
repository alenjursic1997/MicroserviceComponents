using ConfigCore.common.interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml;
using YamlDotNet.Serialization;

namespace ConfigCore.file
{
	public class FileConfig : ConfigSource
	{
		private const string NAME = "file";
		private const int PRIORITY = 50;
		private string _filePath;
		private readonly ILogger _logger;

		public FileConfig(ILogger logger, string filePath = "config.yaml")
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
			_filePath = filePath;
		}

		public string GetValue(string key)
		{
			string json = "";
			//get extension of file
			string fileExtension = Path.GetExtension(_filePath);

			_logger.LogInformation($"Searching value for key '{key}' in file.");

			try
			{
				//if file is in yaml format
				if (fileExtension == ".yaml" || fileExtension == ".yml" )
				{
					var deserializer = new DeserializerBuilder().Build();
					var r = new StringReader(File.ReadAllText(_filePath));
					var yamlObject = deserializer.Deserialize(r);

					var serializer = new SerializerBuilder()
						.JsonCompatible()
						.Build();

					json = serializer.Serialize(yamlObject);
				}
				//if file is in xml format
				else if (fileExtension == ".xml")
				{
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(File.ReadAllText(_filePath));

					json = JsonConvert.SerializeXmlNode(doc.DocumentElement);
				}
				//if file is in json format
				else if (fileExtension == ".json")
				{
					json = File.ReadAllText(_filePath);
				}

				JObject obj = JObject.Parse(json.ToLower());
				return (string)obj.SelectToken(key.ToLower());
			}
			catch
			{
				_logger.LogError($"There were some problems with reading '{_filePath}'.");
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
			return;
		}
	}
}
