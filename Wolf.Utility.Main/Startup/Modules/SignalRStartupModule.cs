using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolf.Utility.Main.Extensions;

namespace Wolf.Utility.Main.Startup.Modules
{
    public class SignalRStartupModule : IStartupModule
    {
        public SetupRouteDelegate SetupRoute { get; }
        public delegate void SetupRouteDelegate(HubRouteBuilder builder);

        public SignalRStartupModule(SetupRouteDelegate setup)
        {
            SetupRoute = setup ?? throw new ArgumentNullException(nameof(setup));
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseSignalR(route => SetupRoute?.Invoke(route));
        }
    }
}
