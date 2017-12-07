using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using TestApp.Proxy;

namespace TestApp
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthenticationOptions>(configuration.GetSection("TestApp:Authentication:AzureAd"));
            services.Configure<TestServiceOptions>(configuration.GetSection("TestApp:TestService"));
            services.AddScoped<TestServiceProxy>();

            services.AddMvc();

            var serviceProvider = services.BuildServiceProvider();
            var authOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>();
            var serviceOptions = serviceProvider.GetService<IOptions<TestServiceOptions>>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = authOptions.Value.Authority;
                options.ClientId = authOptions.Value.ClientId;
                options.ClientSecret = authOptions.Value.ClientSecret;

                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.SignedOutRedirectUri = authOptions.Value.PostLogoutRedirectUri;

                // it will fall back on using DefaultSignInScheme if not set
                //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                options.Events = CreateOpenIdConnectEventHandlers(authOptions.Value, serviceOptions.Value);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<AuthenticationOptions> authOptions, IOptions<TestServiceOptions> serviceOptions)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static OpenIdConnectEvents CreateOpenIdConnectEventHandlers(AuthenticationOptions authOptions, TestServiceOptions serviceOptions) 
            => new OpenIdConnectEvents
            {
                OnAuthorizationCodeReceived = async context =>
                {
                    var clientCredential = new ClientCredential(authOptions.ClientId, authOptions.ClientSecret);
                    var authenticationContext = new AuthenticationContext(authOptions.Authority);
                    var result = await authenticationContext.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code,
                        new Uri(context.TokenEndpointRequest.RedirectUri, UriKind.RelativeOrAbsolute), clientCredential, serviceOptions.Resource);

                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                }
            };
    }
}
