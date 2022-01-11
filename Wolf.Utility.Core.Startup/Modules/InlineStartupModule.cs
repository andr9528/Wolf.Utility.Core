using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Startup.Modules
{
    public class InlineStartupModule : IStartupModule
    {
        public delegate void SetupServicesDelegate(IServiceCollection services);
        private SetupServicesDelegate Setup { get;}

        public delegate void ConfigureApplicationDelegate(IApplicationBuilder app);
        private ConfigureApplicationDelegate Configure;


        public InlineStartupModule(SetupServicesDelegate setupServices = null, ConfigureApplicationDelegate configureApplication = null)
        {
            if (setupServices == null && configureApplication == null) 
                throw new ArgumentNullException(
                    $"Both {setupServices} and {configureApplication} are null, which is not valid for calls to this constructor.");

            Setup = setupServices;
            Configure = configureApplication;
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            Configure?.Invoke(app);
        }

        public void SetupServices(IServiceCollection services)
        {
            Setup?.Invoke(services);
        }
    }
}
