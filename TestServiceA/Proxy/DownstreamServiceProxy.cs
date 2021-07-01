using Contracts;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using System.Net;
using System.Threading.Tasks;

namespace TestServiceA.Proxy
{
    public class DownstreamServiceProxy
    {
        private readonly IDownstreamWebApi downstreamWebApi;

        public DownstreamServiceProxy(IDownstreamWebApi downstreamWebApi)
        {
            this.downstreamWebApi = downstreamWebApi;
        }

        public async Task<ClaimSet> GetClaimSetAsync()
        {
            var response = await downstreamWebApi.CallWebApiForUserAsync(
                        "TestServiceB",
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
