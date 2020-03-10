using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolf.Utility.Main.Persistence.EntityFramework.Core;
using Wolf.Utility.Main.Startup;

namespace Wolf.Utility.Main.Persistence.EntityFramework
{
    public class EntityFrameworkStartupModule<TContext, THandler> : IStartupModule where TContext : DbContext where THandler : class, IHandler
    {
        public IConfiguration Configuration { get; }
        public string ConnectionStringName { get; }

        public EntityFrameworkStartupModule(IConfiguration configuration, string connectionStringName)
        {
            Configuration = configuration;
            ConnectionStringName = connectionStringName;
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddDbContext<TContext>(option =>
                option.UseSqlServer(Configuration.GetConnectionString(ConnectionStringName)));

            services.AddTransient<IHandler, THandler>();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            
        }
    }
}
