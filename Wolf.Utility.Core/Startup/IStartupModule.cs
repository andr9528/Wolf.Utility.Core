using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;


namespace Wolf.Utility.Core.Startup
{
    public interface IStartupModule
    {
        void SetupServices(IServiceCollection services);
        void ConfigureApplication(IApplicationBuilder app);
    }
}
