using ConfigCore.file;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tests.ConfigCoreTests
{
	[TestFixture]
	public class FileConfigTest
	{
        private FileConfig _jsonFileConfig;
        private FileConfig _xmlFileConfig;
        private FileConfig _yamlFileConfig;

        [SetUp]
        public void Init()
        {
            ILogger nullableLogger = NullLogger.Instance;

            _jsonFileConfig = new FileConfig(nullableLogger, Path.GetFullPath(@"..\..\..\ConfigCoreTests\TestConfig.json"));
            _xmlFileConfig = new FileConfig(nullableLogger, Path.GetFullPath(@"..\..\..\ConfigCoreTests\TestConfig.xml"));
            _yamlFileConfig = new FileConfig(nullableLogger, Path.GetFullPath(@"..\..\..\ConfigCoreTests\TestConfig.yaml"));
        }

        [Test]
        public void GetValuesFromJson()
        {
            var res01 = _jsonFileConfig.GetValue("kumuluzee.name");
            var res02 = _jsonFileConfig.GetValue("kumuluzee.server.base-url");
            var res03 = _jsonFileConfig.GetValue("kumuluzee.env.name");
            var res04 = _jsonFileConfig.GetValue("kumuluzee.name.unexisting");

            Assert.AreEqual(res01, "test-service");
            Assert.AreEqual(res02, "http://localhost:9000");
            Assert.AreEqual(res03, "dev");
            Assert.IsNull(res04);
        }

        [Test]
        public void GetValuesFromXml()
        {
            var res01 = _xmlFileConfig.GetValue("kumuluzee.name");
            var res02 = _xmlFileConfig.GetValue("kumuluzee.server.http.port");
            var res03 = _xmlFileConfig.GetValue("kumuluzee.env.name");
            var res04 = _xmlFileConfig.GetValue("kumuluzee.env.unexisting");

            Assert.AreEqual(res01, "test-service");
            Assert.AreEqual(res02, "9000");
            Assert.AreEqual(res03, "dev");
            Assert.IsNull(res04);
        }

        [Test]
        public void GetValuesFromYaml()
        {
            var res01 = _yamlFileConfig.GetValue("kumuluzee.NAME");
            var res02 = _yamlFileConfig.GetValue("kumuluzee.config.namespace");
            var res03 = _yamlFileConfig.GetValue("kumuluzee.env.name");
            var res04 = _yamlFileConfig.GetValue("kumuluzee.discovery.ttl");
            var res05 = _yamlFileConfig.GetValue("kumuluzee.unexisting");

            Assert.AreEqual(res01, "test-service");
            Assert.AreEqual(res02, "");
            Assert.AreEqual(res03, "dev");
            Assert.AreEqual(res04, "100");
            Assert.IsNull(res05);
        }
    }
}
