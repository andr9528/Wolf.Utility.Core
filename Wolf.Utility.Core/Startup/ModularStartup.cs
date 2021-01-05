using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolf.Utility.Core.Logging.Enum;
using static Wolf.Utility.Core.Logging.Logging;

namespace Wolf.Utility.Core.Startup
{
    public abstract class ModularStartup
    {
        public IConfiguration Configuration { get; private set; }
        private readonly List<IStartupModule> _modules;

        /// <summary>
        /// Remember to call 'SetupServices' during 'ConfigureServices' and 'ConfigureApplication' during 'Configure'!
        /// Make use of the 'AddModule' method during the constructor of the Startup, to create and add necessary modules for the project.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="modules"></param>
        protected ModularStartup(IConfiguration config, params IStartupModule[] modules)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            _modules = modules.Length == 0 ? new List<IStartupModule>() : modules.ToList();
        }

        protected void AddModule(IStartupModule module)
        {
            _modules.Add(module);
        }

        protected void SetupServices(IServiceCollection services)
        {
            Log(LogType.Information, $"Setting up Services...");

            foreach (var module in _modules)
            {
                module.SetupServices(services);
            }
        }

        protected void SetupApplication(IApplicationBuilder app)
        {
            Log(LogType.Information, $"Configuring Application...");

            foreach (var module in _modules)
            {
                module.ConfigureApplication(app);
            }
        }
    }
}
