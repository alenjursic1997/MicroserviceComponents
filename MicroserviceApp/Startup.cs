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

			services.AddKumuluzHealth(options =>
			{
				options.RegisterHealthCheck("http_health_check", new HttpHealthCheck("https://github.com/"));
				options.RegisterHealthCheck("as", new DiskSpaceHealthCheck(500, SpaceUnit.Gigabyte));
			});

			services.AddKumuluzConfig(options =>
			{
				options.SetConfigFilePath(Path.GetFullPath("config.yaml"));
				options.SetExtensions(Extension.Consul, Extension.Etcd);
			});


			services.AddKumuluzDiscovery(opt =>
            {
                opt.SetConfigFilePath(Path.GetFullPath("config.yaml"));
                opt.SetExtension(DiscoveryExtension.Consul);
            });
            DiscoveryProvider.GetDiscovery().RegisterService();
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

			app.UseKumuluzHealth("health");
		}
	}
}
