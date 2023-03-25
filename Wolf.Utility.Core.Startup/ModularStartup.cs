using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Wolf.Utility.Core.Startup
{
    public abstract class ModularStartup
    {
        public IConfiguration Configuration { get; private set; }
        public IServiceCollection Services { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }
        private readonly List<IStartupModule> _modules;

        /// <summary>
        /// Remember to call 'ConfigureServices' during 'ConfigureServices' and 'ConfigureApplication' during 'Configure'!
        /// Make use of the 'AddModule' method during the constructor of the Startup, to create and add necessary modules for the project.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="modules"></param>
        protected ModularStartup(IConfiguration config, params IStartupModule[] modules)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            _modules = modules.Length == 0 ? new List<IStartupModule>() : modules.ToList();
        }

        /// <summary>
        /// Expects a 'appsettings.json' file in the main directory.
        /// Remember to call 'ConfigureServices' during 'ConfigureServices' and 'ConfigureApplication' during 'Configure'!
        /// Make use of the 'AddModule' method during the constructor of the Startup, to create and add necessary modules for the project.
        /// </summary>
        /// <param name="modules"></param>
        protected ModularStartup(params IStartupModule[] modules)
        {
            var filename = "appsettings.json";
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), filename))) throw new FileNotFoundException($"Missing 'appsettings.json' in main directory at {Path.Combine(Directory.GetCurrentDirectory(), filename)}");

            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile(filename, optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            _modules = modules.Length == 0 ? new List<IStartupModule>() : modules.ToList();
        }

        protected void AddModule(IStartupModule module)
        {
            _modules.Add(module);
        }

        protected void SetupServices(IServiceCollection services = null)
        {
            if (services == null) 
            {
                var serviceCollection = new ServiceCollection();
                services = serviceCollection;
            }

            Services = services;

            foreach (IStartupModule module in _modules)
            {
                module.ConfigureServices(services);
            }

            ServiceProvider = Services.BuildServiceProvider();
        }

        protected void SetupApplication(IApplicationBuilder app = null)
        {
            app ??= new ApplicationBuilder(ServiceProvider);

            foreach (var module in _modules)
            {
                module.ConfigureApplication(app);
            }
        }
    }
}
