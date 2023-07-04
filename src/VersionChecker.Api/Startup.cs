using Microsoft.OpenApi.Models;
using Serilog;
using VersionChecker.Api.Areas.DotNet;
using VersionChecker.Api.Areas.DotNet.Models;
using VersionChecker.Infrastructure;

namespace VersionChecker.Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers();                
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "VersionChecker.Api", Version = "v1" }));

            services.AddScoped<IRepository<DotNetVersionDetail>, DotNetRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Test"))
            {
                //app.UseExceptionHandler("/error-development");

                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "VersionChecker.Api v1"));
            }
            else
            {
                // app.UseExceptionHandler("/error");
                app.UseSerilogRequestLogging();
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
