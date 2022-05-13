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

        private string ConfigClientId;
        private string ConfigClientSecret;
        private string ConfigClientApiKey;

        public GoogleLoginStartupModule(IConfiguration config)
        {
            ConfigClientId = config.GetValue(ClientIdConfigFieldName, default(string));
            ConfigClientSecret = config.GetValue(ClientSecretConfigFieldName, default(string));
            ConfigClientApiKey = config.GetValue(ClientApiKeyConfigFieldName, default(string));

            if (ConfigClientId == default) 
                throw new MissingConfigFieldException($"Config field with name '{ClientIdConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the OAuth Client Id from Google Cloud");
            if (ConfigClientSecret == default)
                throw new MissingConfigFieldException($"Config field with name '{ClientSecretConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the OAuth Client Secret from Google Cloud");
            if (ConfigClientApiKey == default)
                throw new MissingConfigFieldException($"Config field with name '{ClientApiKeyConfigFieldName}' is missing from appsettings. " +
                    $"Field should contain the Api Key from Google Cloud");

        }

        public void ConfigureApplication(IApplicationBuilder app)
        {
            
        }

        public void SetupServices(IServiceCollection services)
        {
            services.AddSingleton(new GoogleProxy(ConfigClientId, ConfigClientSecret, ConfigClientApiKey));
        }
    }
}
