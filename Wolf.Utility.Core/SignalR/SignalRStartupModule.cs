using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.SignalR
{
    public class SignalRStartupModule : IStartupModule
    {
        // TO BE FIXED ON A LATER DATE

        //public SetupRouteDelegate SetupRoute { get; }
        //public delegate void SetupRouteDelegate(HubRouteBuilder builder);

        //public SignalRStartupModule(SetupRouteDelegate setup)
        //{
        //    SetupRoute = setup ?? throw new ArgumentNullException(nameof(setup));
        //}

        public void SetupServices(IServiceCollection services)
        {
            services.AddSignalR();
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            //app.UseSignalR(route => SetupRoute?.Invoke(route));
        }
    }
}
