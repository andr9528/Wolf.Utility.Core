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
        private readonly List<Hub> hubs;

        public SignalRStartupModule(params Hub[] hubs)
        {
            this.hubs = hubs.ToList();
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            app.UseSignalR(route =>
            {
                foreach (var hub in hubs)
                {
                    route.MabHub($"/{hub.GetType().Name}", hub);
                }
            });
        }
    }
}
