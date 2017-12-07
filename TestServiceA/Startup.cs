using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TestServiceA.Proxy;
using Microsoft.AspNetCore.Http;

namespace TestServiceA
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
            services.Configure<AuthenticationOptions>(configuration.GetSection("TestServiceA:Authentication:AzureAd"));
            services.Configure<DownstreamServiceProxyOptions>(configuration.GetSection("TestServiceA:DownstreamService"));
            services.AddScoped<DownstreamServiceProxy>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc();

            var serviceProvider = services.BuildServiceProvider();
            var authOptions = serviceProvider.GetService<IOptions<AuthenticationOptions>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = authOptions.Value.Authority;
                    options.Audience = authOptions.Value.Audience;

                    options.SaveToken = true;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
