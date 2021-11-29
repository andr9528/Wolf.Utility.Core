using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Xamarin.Startup
{
    public class OouiStartupModule : IStartupModule
    {
        public SetupXamarinDelegate SetupXamarin { get; }
        public delegate void SetupXamarinDelegate();

        public OouiStartupModule(SetupXamarinDelegate setup)
        {
            SetupXamarin = setup;
        }

        public void SetupServices(IServiceCollection services)
        {
            
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseOoui();

            SetupXamarin?.Invoke();
        }
    }
}
