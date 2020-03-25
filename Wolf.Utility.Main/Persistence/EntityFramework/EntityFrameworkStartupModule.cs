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
        private readonly bool migrateOnStartup;
        public SetupOptionsDelegate SetupOptions { get; }
        public delegate void SetupOptionsDelegate(DbContextOptionsBuilder options);

        public EntityFrameworkStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true)
        {
            this.migrateOnStartup = migrateOnStartup;
            SetupOptions = setup ?? throw new ArgumentNullException(nameof(setup));
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddDbContext<TContext>(option => SetupOptions?.Invoke(option));

            services.AddTransient<IHandler, THandler>();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            if (migrateOnStartup)
            {
                using (var service = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var context = service.ServiceProvider.GetService<TContext>())
                    {
                        context.Database.Migrate();
                    }
                } 
            }
        }
    }
}
