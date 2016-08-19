using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Contracts;

namespace TestServiceB.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClaimsController : Controller
    {
        [HttpGet]
        public ClaimSet Get() => new ClaimSet
        {
            ServiceName = "TestServiceB",
            Claims = User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value)
        };
    }
}
