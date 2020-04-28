using ConfigCore.common.converters;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.config;
using ConfigCore.consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tests.ConfigCoreTests.MockingObjects;

namespace Tests.ConfigCoreTests
{
	public class ConsulConfigTest
	{
        private ConsulConfig _config;

        [SetUp]
        public void Init()
        {
            var opt = new ConfigOptions();
            opt.SetConfigFilePath(Path.GetFullPath(@"..\..\..\ConfigCoreTests\TestConfig.yaml"));

            ILogger nullableLogger = NullLogger.Instance;
            IConfig config = new Config(opt);

            _config = new ConsulConfig(config, nullableLogger, new Converter(nullableLogger));

            _config.client = new DummyConsulClient();
        }

        [Test]
        public void GetValues()
        {
            Assert.AreEqual("value1", _config.GetValue("key.test1"));
            Assert.AreEqual(null, _config.GetValue("unexisting"));
        }
    }
}
