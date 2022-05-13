using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.PeopleService.v1;
using Google.Apis.Util;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RestSharp;
using RestSharp.Authenticators;

using Wolf.Utility.Core.Authentication.GoogleInteraction.ProfileReponse;
using Wolf.Utility.Core.Extensions.Methods;
using Wolf.Utility.Core.Logging;

namespace Wolf.Utility.Core.Authentication.GoogleInteraction
{
    // https://developers.google.com/api-client-library/dotnet/guide/aaa_oauth
    public class GoogleProxy
    {
        private string ConfigClientId;
        private string ConfigClientSecret;
        private string ConfigClientApiKey;

        private UserCredential Credential = default;
        private TokenResponse Token = default;
        private IEnumerable<ValidScopes> Scopes = new List<ValidScopes>();
        private bool IsTokenExpired => DateTime.UtcNow - Token.IssuedUtc > TimeSpan.FromMinutes(30);
        private bool IsTokenSet => Token != default;
        private bool IsRefreshTokenPresent => Token.RefreshToken != default;

        public GoogleProxy(string clientId, string clientSecret, string clientKey)
        {
            ConfigClientId = clientId;
            ConfigClientSecret = clientSecret;
            ConfigClientApiKey = clientKey;
        }

        public async Task Login(IEnumerable<ValidScopes> validScopes, ILoggerManager logger = default)
        {
            validScopes = validScopes.Distinct();
            var scopes = GoogleScopes.EnumToString(validScopes);

            var clientSecrets = new ClientSecrets() { ClientId = ConfigClientId, ClientSecret = ConfigClientSecret };

            if (!IsTokenSet || !Scopes.EnsureContains(validScopes))
            {
                Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(clientSecrets, scopes, "user", CancellationToken.None);
                Token = Credential.Token;
            }
            if (IsTokenSet && IsTokenExpired && IsRefreshTokenPresent)
                await GoogleWebAuthorizationBroker.ReauthorizeAsync(Credential, CancellationToken.None);

            Scopes = validScopes;
        }

        public async Task<GoogleProfile> GetProfileInfo(ILoggerManager logger = default)
        {
            try
            {
                if (IsTokenSet && IsTokenExpired || !Scopes.EnsureContains(new[] { ValidScopes.UserInfoProfile }))
                    await Login(Scopes.Concat(new[] { ValidScopes.UserInfoProfile }));

                var client = new RestClient("https://people.googleapis.com/v1/people/me");
                client.AddDefaultHeader("Authorization", $"Bearer {Token.AccessToken}");
                //client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(Token.AccessToken, "Bearer");

                var request = new RestRequest();
                request.AddQueryParameter("personFields", "names");
                request.AddQueryParameter("key", ConfigClientApiKey);

                var response = await client.GetAsync<string>(request);
                Root json = JsonConvert.DeserializeObject<Root>(response);
                var profile = new GoogleProfile() { Email = $"N/A", Name = json.names.FirstOrDefault().displayName, Id = json.names.FirstOrDefault().metadata.source.id, Picture = $"N/A", Verified_Email = $"N/A" };

                return profile;
            }
            catch (Exception e)
            {
                logger?.LogError(e.ToString());
                throw;
            }
        }
    }

    public enum ValidScopes
    {
        UserInfoProfile
    }

    internal class GoogleScopes
    {
        private const string UserInfoProfile = PeopleServiceService.ScopeConstants.UserinfoProfile;

        private static GoogleScopes instance = default;

        private GoogleScopes()
        {
        }

        internal static GoogleScopes GetInstance()
        {
            if (instance == default) instance = new GoogleScopes();
            return instance;
        }

        internal static IEnumerable<string> EnumToString(IEnumerable<ValidScopes> scopes)
        {
            var result = new List<string>();

            var constants = typeof(GoogleScopes).GetConstants(BindingFlags.NonPublic |
                BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var scope in scopes)
            {
                var found = constants.Where(x => x.Name == scope.ToString());
                if (found.Any())
                {
                    var field = found.First();
                    result.Add(field.GetValue(GetInstance()).ToString());
                }
            }
            return result;
        }
    }
}