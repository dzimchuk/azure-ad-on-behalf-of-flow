using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TestServiceA.Proxy;

namespace TestServiceA
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
            services.Configure<AuthenticationOptions>(Configuration.GetSection("TestServiceA:Authentication:AzureAd"));
            services.Configure<DownstreamServiceProxyOptions>(Configuration.GetSection("TestServiceA:DownstreamService:BaseUrl"));
            services.AddScoped<DownstreamServiceProxy>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<AuthenticationOptions> authOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,

                Authority = authOptions.Value.Authority,
                Audience = authOptions.Value.Audience,

                SaveToken = true,
                
                Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.SkipToNextMiddleware();
                        return Task.FromResult(0);
                    }
                }
            });

            app.UseMvc();
        }
    }
}
