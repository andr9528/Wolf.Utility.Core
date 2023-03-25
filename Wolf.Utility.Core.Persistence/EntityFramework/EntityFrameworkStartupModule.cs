using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    public class EntityFrameworkStartupModule<TContext, THandler, TEntity, TSearchable> : IStartupModule
        where TContext : DbContext
        where THandler : class, IHandler<TEntity, TSearchable>
        where TEntity : class, IEntity
        where TSearchable : class, ISearchableEntity
    {
        private readonly bool migrateOnStartup;
        public SetupOptionsDelegate SetupOptions { get; }

        public delegate void SetupOptionsDelegate(DbContextOptionsBuilder options);

        public EntityFrameworkStartupModule(SetupOptionsDelegate setup, bool migrateOnStartup = true)
        {
            this.migrateOnStartup = migrateOnStartup;
            SetupOptions = setup ?? throw new ArgumentNullException(nameof(setup));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TContext>(option => SetupOptions?.Invoke(option));

            services.AddTransient<IHandler<TEntity, TSearchable>, THandler>();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            if (migrateOnStartup)
            {
                using IServiceScope service =
                    app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
                using var context = service.ServiceProvider.GetService<TContext>();
                context.Database.Migrate();
            }
        }
    }
}