using ConfigCore.common.interfaces;
using ConfigCore.environment;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.ConfigCoreTests
{
	[TestFixture]
	public class EnvironmentConfigTest
	{
        ILogger nullableLogger;
        ConfigSource config;

        [SetUp]
        public void Init()
        {
            nullableLogger = NullLogger.Instance;
            config = new EnvConfig(nullableLogger);

            Environment.SetEnvironmentVariable("kumuluzee-name", "test-service", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("kumuluzee-server-http-port", "9000", EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable("kumuluzee.name.dot", "test-service_dot", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("kumuluzee.server.http.port.dot", "9000_dot", EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable("kumuluzee_name_underscore", "test-service_underscore", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("kumuluzee_server_http_port_underscore", "9000_underscore", EnvironmentVariableTarget.Process);

            Environment.SetEnvironmentVariable("KUMULUZEE_NAME_UPPERCASE", "test-service_uppercase", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("KUMULUZEE_SERVER_HTTP_PORT_UPPERCASE", "9000_uppercase", EnvironmentVariableTarget.Process);
        }

        [Test]
        public void GetDotValue()
        {
            var res01 = config.GetValue("kumuluzee.name.dot");
            var res02 = config.GetValue("kumuluzee.server.http.port.dot");
            var res03 = config.GetValue("kumuluzee.name");

            Assert.AreEqual(res01, "test-service_dot");
            Assert.AreEqual(res02, "9000_dot");
            Assert.IsNull(res03);
        }

        [Test]
        public void GetUnderscoreValue()
        {
            var res01 = config.GetValue("kumuluzee.name.underscore");
            var res02 = config.GetValue("kumuluzee.server.http.port.underscore");

            Assert.AreEqual(res01, "test-service_underscore");
            Assert.AreEqual(res02, "9000_underscore");
        }

        [Test]
        public void GetUppercaseValue()
        {
            var res01 = config.GetValue("kumuluzee.name.uppercase");
            var res02 = config.GetValue("kumuluzee.server.http.port.uppercase");
            var res03 = config.GetValue("kumuluzee.server.http.port");

            Assert.AreEqual(res01, "test-service_uppercase");
            Assert.AreEqual(res02, "9000_uppercase");
            Assert.IsNull(res03);
        }
    }
}
