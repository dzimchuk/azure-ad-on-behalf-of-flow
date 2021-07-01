using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Models;
using TestApp.Proxy;

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
                InnerClaimSet = await proxy.GetClaimSetAsync()
            };

            return View(claimSet);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
