# KumuluzEE Health

> KumuluzEE Health project provides consistent, unified way of performing microservice health checks and exposing health
  information.

KumuluzEE Health is a health check project for the KumuluzEE microservice framework. It provides easy, consistent and
unified way of performing health checking on microservices and exposing health information to be used by monitoring and
container orchestration environments such as Kubernetes.

KumuluzEE Health is compliant with the [MicroProfile Service Health Checks specification 2.2](https://github.com/eclipse/microprofile-health).

## Install
This library can be installed trough nuget package via nuget package manager console as shown below:

```cmd
PM> Install-Package Kumuluzee.Health
```
## Usage
KumuluzEE Health can be used in your application with dependency injection.

Registration:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddKumuluzHealth();
}
```

To actually enable it in your service you must call `UseKumuluzHealth()` method on `IApplicationBuilder` interface:
```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseKumuluzHealth("health");
}
```

`UseKumuluzHealth()` method accepts optional string parameter `route` which is used as a part of URL path where health status will be accessible.
Default value of route is `/health`. In case you don't pass this parameter, health state of service is going to be available on URL: `<YOUR_SERVICE_URL>/health`

## Health checks

To check health of a microservice, you can use the provided health checks or you can define your own health checks.

## Liveness and readiness

KumuluzEE Health differentiates between two health check types - liveness and readiness health check. In short - if a
liveness check fails it means that the service is stuck and should be restarted. If a readiness check fails it means
that the service is temporary unavailable and should not receive requests until all readiness checks succeed.

For more information on liveness and readiness see the following Kubernetes articles:

- [Configure Liveness and Readiness Probes](https://kubernetes.io/docs/tasks/configure-pod-container/configure-liveness-readiness-probes/)
- [Container probes](https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle/#container-probes)

## Built-in health checks

The following health checks are available out-of-the-box: 

- **DiskSpaceHealthCheck** for checking available disk space against a threshold
- **HttpHealthCheck** for checking the availability of HTTP resource
- **RedisHealthCheck** for checking the availability of Redis store
- **SqlServerHealthCheck** for checking the availability of Sql server

Additional built-in health check will be provided soon.

## Implementing custom health checks

To implement your custom health check you must create a new class that derives from `HealthCheck` abstract class.
It includes method `CheckResponse()` which returns object of type `HealthCheckReponse`. On every health check implementation we can also use `Liveness` or `Readiness` attributes.

`HealthCheckResponse` contains the following properties:
- **Status** enumeration whith values "DOWN" or "UP", which can be set by calling methods `Up()` and `Down()` on HealthCheckReponse type object.
- **Name** for naming health check.
- **Data** type of `object` where any health check informations can be returned.

### @Liveness and @Readiness attributes

Responses of health checks with attributes `Readiness` or `Liveness` can be accessed on:
- `<YOUR_SERVICE_URL>/<ROUTE_PARAMETER>/ready` where only health checks with attribute `Readiness` are shown 
- `<YOUR_SERVICE_URL>/<ROUTE_PARAMETER>/live` where only health checks with attribute `Liveness` are shown


### Registering health checks

To register a custom health check class we have to use the `Health` (in our case `_health`) instance which you can get in constructor via dependency injection. We provide the health check unique
name and an instance of the health check class.
Example:
```csharp
_health.Register("name_of_health_check_instace", new MyCustomHealthCheck(...));
```

Health check can also be registered when registering KumuluzEE Health.
Example:
```csharp
services.AddKumuluzHealth(options =>
{
	  options.RegisterHealthCheck("http_health_check", new HttpHealthCheck("https://github.com/"));
	  options.RegisterHealthCheck("dist_space_health_check", new DiskSpaceHealthCheck(500, SpaceUnit.Gigabyte));
});
```

### Unregistering health checks

To unregister health checks we can use the `Health` instance and provide the health check unique name.

```java
_health.Unregister("name_of_health_check_instace");
```

## /health/* endpoint output

The `/health`, `/health/live` and `/health/ready` endpoints return:

- 200 with payload, when health checks are defined with positive status or are not defined
- 503 with payload, when health checks are defined, but at least one status is negative
- 500 without payload, when an exception occurred in the procedure of health checking

The health check is available on `http://IP:PORT/health/live` and `http://IP:PORT/health/ready` by default, payload
example is provided below:

```json
{
  "status" : "UP",
  "checks" : [ {
    "name" : "DiskSpaceHealthCheck",
    "status" : "UP"
  }, {
    "name" : "HttpHealthCheck",
    "status" : "UP",
    "data": {
      "https://github.com/kumuluz/kumuluzee-health": "UP"
    }
  }, {
    "name" : "RedisHealthCheck",
    "status" : "UP"
  } ]
}
```

## License

MIT
