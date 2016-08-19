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
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();

            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (o, certificate, chain, errors) => true;
        }

        private IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AuthenticationOptions>(Configuration.GetSection("TestApp:Authentication:AzureAd"));
            services.Configure<TestServiceOptions>(Configuration.GetSection("TestApp:TestService"));
            services.AddScoped<TestServiceProxy>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<AuthenticationOptions> authOptions, IOptions<TestServiceOptions> serviceOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AutomaticChallenge = true,

                Authority = authOptions.Value.Authority,
                ClientId = authOptions.Value.ClientId,
                ClientSecret = authOptions.Value.ClientSecret,

                ResponseType = OpenIdConnectResponseType.CodeIdToken,

                SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                PostLogoutRedirectUri = authOptions.Value.PostLogoutRedirectUri,

                Events = CreateOpenIdConnectEventHandlers(authOptions.Value, serviceOptions.Value)
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static IOpenIdConnectEvents CreateOpenIdConnectEventHandlers(AuthenticationOptions authOptions, TestServiceOptions serviceOptions) => new OpenIdConnectEvents
        {
            OnAuthorizationCodeReceived = async context =>
            {
                var clientCredential = new ClientCredential(authOptions.ClientId, authOptions.ClientSecret);
                var authenticationContext = new AuthenticationContext(authOptions.Authority);
                await authenticationContext.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code,
                    new Uri(context.TokenEndpointRequest.RedirectUri, UriKind.RelativeOrAbsolute), clientCredential, serviceOptions.Resource);

                context.HandleCodeRedemption();
            }
        };
    }
}
