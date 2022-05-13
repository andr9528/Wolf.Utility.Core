using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Extensions.Methods;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.Web.Startup
{
    public class ProxyStartupModule<TProxy, TEntity> : IStartupModule where TProxy : ControllerProxy where TEntity : class, IEntity
    {
        const string BaseAddressConfigFieldName = "BaseAddress";

        private readonly string controller;
        private readonly bool useHandlerConstructor;
        private readonly string baseAddress;

        /// <summary>
        /// If useHandlerConstructor is set to true, this should be added after Entity Framework Startup module.
        /// </summary>
        /// <param name="useHandlerConstructor"></param>
        public ProxyStartupModule(IConfiguration config, string controller, bool useHandlerConstructor = false)
        {
            this.controller = controller;
            this.useHandlerConstructor = useHandlerConstructor;

            baseAddress = config.GetValue(BaseAddressConfigFieldName, default(string));
            if (string.IsNullOrWhiteSpace(baseAddress)) throw new ArgumentNullException($"Failed to retrieve a valid string from Application Settings - {nameof(baseAddress)}");
        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            
        }

        public void SetupServices(IServiceCollection services)
        {
            TProxy proxy = null;
            if (useHandlerConstructor)
            {
                var provider = services.BuildServiceProvider();
                var handler = provider.GetService<IHandler>();
                
                proxy = TypeExtensions.CreateInstance<TProxy>(baseAddress, controller, handler);
            }
            else 
            {
                proxy = TypeExtensions.CreateInstance<TProxy>(baseAddress, controller);
            }
            
            services.AddSingleton((IEntityControllerProxy<TEntity>)proxy);
        }
    }
}
