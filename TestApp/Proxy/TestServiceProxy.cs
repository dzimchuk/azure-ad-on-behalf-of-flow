using Contracts;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace TestApp.Proxy
{
    public class TestServiceProxy
    {
        private readonly IDownstreamWebApi downstreamWebApi;

        public TestServiceProxy(IDownstreamWebApi downstreamWebApi)
        {
            this.downstreamWebApi = downstreamWebApi;
        }

        public async Task<ClaimSet> GetClaimSetAsync()
        {
            var response = await downstreamWebApi.CallWebApiForUserAsync(
                        "TestServiceA",
                        options =>
                        {
                            options.RelativePath = "api/claims";
                        });

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ClaimSet>(content);
            }

            return null;
        }
    }
}
