using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TestApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(
                        OpenIdConnectDefaults.AuthenticationScheme,
                        new AuthenticationProperties
                        {
                            RedirectUri = "/"
                        });
            }

            return RedirectHome();
        }

        public async Task<IActionResult> SignOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

                return new EmptyResult();
            }

            return RedirectHome();
        }

        private IActionResult RedirectHome() => RedirectToAction(nameof(HomeController.Index), "Home");
    }
}
