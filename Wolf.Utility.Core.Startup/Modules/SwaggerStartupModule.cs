using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Wolf.Utility.Core.Startup.Modules
{
    public class SwaggerStartupModule : IStartupModule
    {
        public SwaggerStartupModule()
        {
            
        }

        public void SetupServices(IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
