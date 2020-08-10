using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HealthCore.Extensions;
using DiscoveryCore.extensions;
using DiscoveryCore.discovery;
using DiscoveryCore.common.models;
using System.IO;
using HealthCore.Checks;
using ConfigCore.extensions;
using ConfigCore.common.models;

namespace MicroserviceApp
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			//services.AddKumuluzHealth(options =>
			//{
			//	options.RegisterHealthCheck("http_health_check", new HttpHealthCheck("https://github.com/"));
			//	options.RegisterHealthCheck("as", new DiskSpaceHealthCheck(500, SpaceUnit.Gigabyte));
			//});

			//services.AddKumuluzConfig(options =>
			//{
			//	options.SetConfigFilePath(Path.GetFullPath("config.yaml"));
			//	options.SetExtensions(Extension.Consul, Extension.Etcd);
			//});


			services.AddKumuluzDiscovery(opt =>
            {
                opt.SetConfigFilePath(Path.GetFullPath("config.yaml"));
                opt.SetExtension(DiscoveryExtension.Consul);
            });
			//DiscoveryProvider.GetDiscovery().RegisterService(new RegisterOptions() { Singleton = true });

            var discoverOptions1 = new DiscoverOptions
            {
                ServiceName = "test-service",
                Environment = "devf",
                AccessType = AccessType.Direct,
                Version = ">1.0.2"
            };
            var res1 = DiscoveryProvider.GetDiscovery().DiscoverService(discoverOptions1);

            var discoverOptions2 = new DiscoverOptions
            {
                ServiceName = "test-service",
                Environment = "devf",
                AccessType = AccessType.Direct,
                Version = "<=1.0.0"
            };
            var res2 = DiscoveryProvider.GetDiscovery().DiscoverService(discoverOptions2);

            var discoverOptions3 = new DiscoverOptions
            {
                ServiceName = "test-service",
                Environment = "devf",
                AccessType = AccessType.Direct,
                Version = "^2.0.2"
            };
            var res3 = DiscoveryProvider.GetDiscovery().DiscoverService(discoverOptions3);

            var discoverOptions4 = new DiscoverOptions
            {
                ServiceName = "test-service",
                Environment = "devf",
                AccessType = AccessType.Direct,
                Version = ">1.0.2"
            };
            var res4 = DiscoveryProvider.GetDiscovery().DiscoverService(discoverOptions4);
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			//app.UseKumuluzHealth("health");
		}
	}
}
