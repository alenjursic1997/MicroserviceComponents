using ConfigCore.common.converters;
using ConfigCore.common.interfaces;
using ConfigCore.common.models;
using ConfigCore.config;
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
    public class ConfigResultTypesTest
    {
        private IConfig _config;

        [SetUp]
        public void Init()
        {
            ILogger nullableLogger = NullLogger.Instance;

            var options = new ConfigOptions();
            options.SetConfigFilePath(@"..\..\..\ConfigCoreTests\TestConfig.yaml");
            _config = new Config(options);


            new FileConfig(nullableLogger, Path.GetFullPath(@"..\..\..\ConfigCoreTests\TestConfig.yaml"));
        }

        [Test]
        public void GetValuesFromJson()
        {
            //integer
            Assert.AreEqual(12, _config.Get<int>("types.int.positive"));
            Assert.AreEqual(-1234, _config.Get<int>("types.int.negative"));
            //string
            Assert.AreEqual("beseda", _config.Get<string>("types.string"));
            //double
            Assert.AreEqual((double)12.34, _config.Get<double>("types.double"));
            //float
            Assert.AreEqual((float)2345.02, _config.Get<float>("types.float"));
            //bool
            Assert.AreEqual(true, _config.Get<bool>("types.bool.true"));
            Assert.AreEqual(false, _config.Get<bool>("types.bool.false"));
            Assert.AreEqual(true, _config.Get<bool>("types.bool.yes"));
            Assert.AreEqual(false, _config.Get<bool>("types.bool.no"));
            Assert.AreEqual(true, _config.Get<bool>("types.bool.y"));
            Assert.AreEqual(false, _config.Get<bool>("types.bool.n"));
            Assert.AreEqual(true, _config.Get<bool>("types.bool.one"));
            Assert.AreEqual(false, _config.Get<bool>("types.bool.zero"));
            //char
            Assert.AreEqual('x', _config.Get<char>("types.char"));
            //array
            Assert.AreEqual(new string[] { "test1", "test2", "test3" }, _config.Get<string[]>("types.array.string"));
            //custom object
            var result = _config.Get<MojObjekt>("types.mojobjekt");
            Assert.AreEqual("ime", result.Name);
            Assert.AreEqual("vrednost", result.Value);
        }
    }

    public class MojObjekt
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class MojObjektConverter : BaseTypeConverter<MojObjekt>
    {
        public override int Priority => 1;

        public override MojObjekt Convert(string value)
        {
            MojObjekt result = new MojObjekt();
            try
            {
                var props = value.Split(';');
                foreach (var prop in props)
                {
                    if (prop.Split('=')[0] == "name")
                        result.Name = prop.Split('=')[1];
                    if (prop.Split('=')[0] == "value")
                        result.Value = prop.Split('=')[1];
                }
            }
            catch {}
            return result;
        }
    }
}
