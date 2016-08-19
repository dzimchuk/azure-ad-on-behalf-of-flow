using Contracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TestServiceA.Proxy
{
    public class DownstreamServiceProxy
    {
        private readonly AuthenticationOptions authOptions;
        private readonly DownstreamServiceProxyOptions serviceOptions;

        public DownstreamServiceProxy(IOptions<AuthenticationOptions> authOptions, IOptions<DownstreamServiceProxyOptions> serviceOptions)
        {
            this.authOptions = authOptions.Value;
            this.serviceOptions = serviceOptions.Value;
        }

        public async Task<ClaimSet> GetClaimSetAsync(string userId)
        {
            var client = new HttpClient { BaseAddress = new Uri(serviceOptions.BaseUrl, UriKind.Absolute) };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync(userId));

            var payload = await client.GetStringAsync("api/claims");
            return JsonConvert.DeserializeObject<ClaimSet>(payload);
        }

        private async Task<string> GetAccessTokenAsync(string userId)
        {
            var credential = new ClientCredential(authOptions.ClientId, authOptions.ClientSecret);
            var authenticationContext = new AuthenticationContext(authOptions.Authority);

            var user = !string.IsNullOrEmpty(userId) ? new UserIdentifier(userId, UserIdentifierType.UniqueId) : UserIdentifier.AnyUser;
            var result = await authenticationContext.AcquireTokenSilentAsync(authOptions.ApiResource, credential, user);

            return result.AccessToken;
        }
    }
}
