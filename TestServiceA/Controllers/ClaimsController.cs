using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Contracts;
using TestServiceA.Proxy;

namespace TestServiceA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        private readonly DownstreamServiceProxy proxy;

        public ClaimsController(DownstreamServiceProxy proxy)
        {
            this.proxy = proxy;
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<ClaimSet> Get() => new ClaimSet
        {
            ServiceName = "TestServiceA",
            Claims = User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value),
            InnerClaimSet = await proxy.GetClaimSetAsync()
        };
    }
}
