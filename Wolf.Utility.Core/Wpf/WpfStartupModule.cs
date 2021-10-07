using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.Wpf
{
    // https://executecommands.com/dependency-injection-in-wpf-net-core-csharp/

    /// <summary>
    /// Adds a Singleton for the TMainWindow supplied.
    /// </summary>
    /// <typeparam name="TMainWindow">Should be the Type of the MainWindow of the Wpf application</typeparam>
    public class WpfStartupModule<TMainWindow> : IStartupModule where TMainWindow : class
    {
        public void ConfigureApplication(IApplicationBuilder app)
        {
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddSingleton<TMainWindow>();
        }
    }
}
