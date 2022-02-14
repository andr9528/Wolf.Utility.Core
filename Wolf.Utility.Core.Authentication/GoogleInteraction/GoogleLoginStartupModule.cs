using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Authentication.GoogleInteraction;
using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Startup;

namespace Wolf.Utility.Core.Authentication.GoogleInteraction
{
    public class GoogleLoginStartupModule : IStartupModule
    {
        const string ClientSecretConfigFieldName = "GoogleClientSecret";
        const string ClientIdConfigFieldName = "GoogleClientId";
        const string ClientApiKeyConfigFieldName = "GoogleApiKey";
        public GoogleLoginStartupModule(IConfiguration config)
        {
            GoogleProxy.ConfigClientId = config.GetValue(ClientIdConfigFieldName, default(string));
            GoogleProxy.ConfigClientSecret = config.GetValue(ClientSecretConfigFieldName, default(string));
            GoogleProxy.ConfigClientApiKey = config.GetValue(ClientApiKeyConfigFieldName, default(string));

            if (GoogleProxy.ConfigClientId == default) 
                throw new MissingConfigFieldException($"Config field with name '{ClientIdConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the OAuth Client Id from Google Cloud");
            if (GoogleProxy.ConfigClientSecret == default)
                throw new MissingConfigFieldException($"Config field with name '{ClientSecretConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the OAuth Client Secret from Google Cloud");
            if (GoogleProxy.ConfigClientApiKey == default)
                throw new MissingConfigFieldException($"Config field with name '{ClientApiKeyConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the Api Key from Google Cloud");

        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddSingleton(new GoogleProxy());
        }
    }
}
