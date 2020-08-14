# KumuluzEE Discovery

> Service discovery project for the KumuluzEE microservice framework. Service registration, service discovery and client side load balancing with full support for Docker and Kubernetes cluster.

KumuluzEE Discovery is a service discovery project for the KumuluzEE microservice framework. It provides support 
for service registration, service discovery and client side load balancing.

KumuluzEE Discovery provides full support for microservices packed as Docker containers. It also provides full support 
for executing microservices in clusters and cloud-native platforms with full support for Kubernetes.
 
KumuluzEE Discovery has been designed to support modularity with pluggable service discovery frameworks. Currently, 
only Consul is supported. In the future, other discovery frameworks will be supported too (contributions are welcome).

## Usage

You can enable discovery by installing nuget package as followed:
```cmd
TODO:
```

### Configuring Consul

Configuration properties can be defined with the environment variables or in the configuration file. For more details see the 
[KumuluzEE configuration wiki page](https://github.com/kumuluz/kumuluzee/wiki/Configuration).

By default, Consul connects to the local agent (`http://localhost:8500`) without additional configuration. You can 
specify the URL of the Consul agent with configuration key `kumuluzee.discovery.consul.hosts`. Note that Consul is 
responsible for assigning the IP addresses to the registered services and will assign them the IP on which the agent is 
accessible. Specifying an agent IP address is therefore useful in specific situations, for example when you are running 
multiple services on single Docker host and want them to connect to the single Consul agent, running on the same Docker 
host. 

If your service is accessible over https, you must specify that with configuration key 
`kumuluzee.discovery.consul.protocol: https`. Otherwise, http protocol is used.

Consul implementation reregisters services in case of errors and sometimes unused services in critical state remain in
Consul. To avoid this, Consul implementation uses Consul parameter `DeregisterCriticalServiceAfter` when registering
services. To read more about this parameter, see Consul documentation: https://www.consul.io/api/agent/check.html#deregistercriticalserviceafter.
To alter the value of this parameter, set configuration key `kumuluzee.discovery.consul.deregister-critical-service-after-s`
appropriately. Default value is 60 (1 min).

Services in Consul are registered with the following name: `'environment'-'serviceName'`

Version is stored in service tag with following format: `version='version'`

If the service uses https protocol, tag `https` is added.

**YAML Configuration**

Example of YAML configuration:

```yaml
kumuluzee:
  name: my-service
  env:
    name: test
  version: 1.2.3
  server:
    http:
      port: 8081
    base-url: http://localhost:8081
  discovery:
    consul:
      hosts: http://127.0.0.1:8500
      deregister-critical-service-after-s: 120
    ttl: 30
    ping-interval: 5
```

### Service registration

Service can be registered as followed:

```csharp
services.AddKumuluzDiscovery(opt =>
{
    opt.SetConfigFilePath(Path.GetFullPath("config.yaml"));
    opt.SetExtension(DiscoveryExtension.Consul);
});
```

It's also possible to set logger. This can be called in ConfigureServices method in Startup class.

Discovery object can be retrieved by dependency injection or via DiscoveryProvider.

```csharp
public MyClass(IDiscovery discovery) {...}
```
or
```csharp
var testValue = ConfigProvider.GetConfig().Get<int>("test.value");
```

When calling RegisterService() method to register service, following parameters can be set within RegisterOptions object.
- ServiceName: a name of our service.
- TTL: time to live of a registration key in the store. Default value is 30 seconds. TTL can be overridden with configuration key `kumuluzee.discovery.ttl`.
- PingInterval: an interval in which service updates registration key value in the store. Default value is 20. Ping interval can be overridden with configuration key `kumuluzee.discovery.ping-interval`.
- Environment: environment in which service is registered. Default value is "dev". Environment can be overridden with configuration key `kumuluzee.env.name`.
- Version: version of service to be registered. Default value is "1.0.0". Version can be overridden with configuration key `kumuluzee.version`.
- Singleton: if true ensures, that only one instance of service with the same name, version and environment is
registered. Default value is false.

Example of service registration:
```csharp
var options = new RegisterOptions
{
    Environment = "dev",
    ServiceName = "test-service",
    PingInterval = 20,
    Version = "1.0.0",
    TTL = 30,
    Singleton = false
};
DiscoveryProvider.GetDiscovery().RegisterService(options);
```


### Service discovery


## License

MIT
