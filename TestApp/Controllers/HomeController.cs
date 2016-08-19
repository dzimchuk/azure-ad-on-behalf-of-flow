using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TestApp.Proxy;
using Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestServiceProxy proxy;

        public HomeController(TestServiceProxy proxy)
        {
            this.proxy = proxy;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Test()
        {
            var claimSet = new ClaimSet
            {
                ServiceName = "TestApp",
                Claims = User.Claims.ToDictionary(claim => claim.Type, claim => claim.Value),
                InnerClaimSet = await proxy.GetClaimSetAsync(User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value)
            };

            return View(claimSet);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
