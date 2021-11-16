using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.Authentication
{
    public class GoogleAuthenticationStartupModule : IStartupModule
    {
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }

        public void SetupServices(IServiceCollection services)
        {
            //services.AddAuthentication().AddGoogle();
        }
    }
}
