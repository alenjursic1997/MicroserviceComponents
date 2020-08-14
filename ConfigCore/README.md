# KumuluzEE .NET Core Config

> Configuration extension for the KumuluzEE microservice framework with support for etcd and Consul configuration 
servers.

KumuluzEE .NET Core Config is an open-source configuration management project for the KumuluzEE framework. It extends basic 
configuration framework which is a part of KumuluzEE framework, described 
[here](https://github.com/kumuluz/kumuluzee/wiki/Configuration). It provides support for additional configuration 
sources in addition to environment variables, system properties and configuration files. 

KumuluzEE .NET Core Config follows the idea of a unified configuration API for the framework and provides additional
configuration sources which can be utilised with a standard KumuluzEE configuration interface. 

KumuluzEE .NET Core Config has been designed to support modularity with pluggable configuration sources. Currently, etcd and 
Consul key-value stores are supported to act as configuration servers. In the future, other data stores and 
configuration servers will be supported too (contributions are welcome).

## Usage
KumuluzEE defines interfaces for common configuration management features and four basic configuration sources; 
environment variables, configuration files, etcd and Consul key-value stores.

Currently, only API v2 is supported. Future releases will support API v3 in a form of a new KumuluzEE Config module.

You can include this functionality by installing nuget package as followed:

```cmd
TODO
```

Note that currently, only one configuration server implementation (etcd or Consul) can be added to a single project.
Adding both of them may result in unexpected behaviour.

**Configuring etcd**

To connect to an etcd cluster, an odd number of etcd hosts must be specified with configuration key `kumuluzee.config
.etcd.hosts` in format 
`'http://192.168.99.100:2379,http://192.168.99.101:2379,http://192.168.99.102:2379'`.

Etcd can be configured to support user authentication and client-to-server transport security with HTTPS. To access 
authentication-enabled etcd host, username and password have to be defined with configuration keys 
`kumuluzee.config.etcd.username` and `kumuluzee.config.etcd.password`. To enable transport security, follow 
https://coreos.com/etcd/docs/latest/op-guide/security.html To access HTTPS-enabled etcd host, PEM certificate string
have to be defined with configuration key `kumuluzee.config.etcd.ca`.

Sample configuration file: 

```yaml
kumuluzee:
  config:
    start-retry-delay-ms: 500
    max-retry-delay-ms: 900000
    etcd:
      hosts: http://192.168.99.100:2379,http://192.168.99.101:2379,http://192.168.99.102:2379
      username: root
      password: admin
      ca: -----BEGIN CERTIFICATE-----
          MIIDDjCCAfagAwIBAgIUZzEIr206GOYqlxHLWtUUEu2ztvcwDQYJKoZIhvcNAQEL
          BQAwDTELMAkGA1UEAxMCQ0EwHhcNMTcwNDEwMDcyMDAwWhcNMjIwNDA5MDcyMDAw
          WjANMQswCQYDVQQDEwJDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB
          AMKAeFREzc3wjOCQ8RlbnTJmD0PUls4HS6lV/xlRKbsNwqC3rxpoSp7lDoVy6MNr
          vX+7ZiyL05bkhWfF6Vzqqy6BVc6ock+nsIQyn1mXaTYDftue2z142KpjPLsj9YbP
          r2C5fmQk3rigQER95nT4gX3SleFENrnsmJU8bOt59b33uaYv6WLKUCInADITsQAN
          O8LiQ4scRwQXMFq0xORWdno9xPoRZOKMi5p+mIN0cGl9/+ComuqIcWomjKkWYK58
          Qhsy9jSaFYo6INMKLAjnmu5qY2Z7Hpf6iaVjgCayO8IXBWegspCTtZWZKOCpbO4A
          w3iH1eCz6VaG3F9FC1yWlh0CAwEAdaNmMGQwDgYDVR0PAQH/BAQDAgEGMBIGA1Ud
          EwEB/wQIMAYBAf8CAQIwHQYDeoklfBYEFBG6m7kZljsfFK2MTnQ5RWdM+mnDMB8G
          A1UdIwQYMBaAFBG6m7kZljsfFK2MTnQ5RWdM+mnDMA0GCSqGSIb3DQEBCwUAA4IB
          AQAT3tRmXGqt8Uh3Va0+Rlm4MDzcFsD7aO77tJuELCDC4cOCeROCEtYNJGm33MFe
          buxwaZ+zAneg5a1DtDkdjMZ6N+CVkMBTDWWm8cuo6Dm3HKWr+Rtd6Z8LwOq/X40C
          CHyowEYlYZSAof9rOHwn0rt8zgUSmZV6z9PXwFajwE2nEU7wlglYXtuLqBNzUYeN
          wYNnVFjMYtsWKgi/3nCegXastYGqoDpnAT25CsExrRuxAQw5i5WJU5RJwNsOPod5
          6X2Iz/EV5flbWti5OcoxLr3pfaCueLa71E+mPDKlWB55BXdNyHyS248msZC7UD2I
          Opyz239QjRq2HRMl+i7C0e6O
          -----END CERTIFICATE-----
```

**Configuring Consul**

By default, KumuluzEE Config Consul automatically connects to the local agent at http://localhost:8500. This behaviour 
can be overridden by specifying agent URL with configuration key `kumuluzee.config.consul.hosts`.

**Configuration source priorities**

Included source acts as any other configuration source. It has the third highest priority, which means that properties 
from etcd override properties from configuration files and can be overwritten with properties system properties.

**Using configuration in service

In Startup class following code should be added inside ConfigureServices method.

```csharp
services.AddKumuluzConfig(options =>
{
    options.SetConfigFilePath(Path.GetFullPath("config.yaml"));
    options.SetExtensions(Extension.Consul, Extension.Etcd);
});
```

It is also possible to set logger for this configuration

**Retrieving configuration properties**

Configuration properties can be retrieved with dependency injection or via ConfigProvider. Example:

```csharp
public MyClass(IConfig config)
{
  var testValue = config.Get<int>("test.value");
}
```
or
```csharp
var testValue = ConfigProvider.GetConfig().Get<int>("test.value");
```

**Watches**

Since configuration properties in etcd and Consul can be updated during microservice runtime, they have to be
dynamically updated inside the running microservices. This behaviour can be enabled by subscribing key.

```csharp
int testValue;
ConfigProvider.GetConfig().Subscribe<int>("test.value", (val) => testValue = val);
}
```

**Retry delays**

Etcd and Consul implementations support retry delays on watch connection errors. Since they use increasing exponential
delay, two parameters need to be specified:

- `kumuluzee.config.start-retry-delay-ms`, which sets the retry delay duration in ms on first error - default: 500
- `kumuluzee.config.max-retry-delay-ms`, which sets the maximum delay duration in ms on consecutive errors -
default: 900000 (15 min)


## License

MIT
