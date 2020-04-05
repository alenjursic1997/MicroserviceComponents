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
			services.AddKumuluzHealth(null);

			services.AddKumuluzDiscovery(opt=>
			{
				opt.SetConfigFilePath(Path.GetFullPath("settings.yaml"));
				opt.SetExtensions("etcd");
			});
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
