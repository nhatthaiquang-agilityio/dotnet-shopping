using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Services;
using WebMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using WebMVC.Infrastructure;

namespace WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the IoC container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomMvc(Configuration)
                    .AddHttpClientServices(Configuration)
                    .AddCustomAuthentication(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                loggerFactory.CreateLogger<Startup>().LogDebug("Using PATH BASE '{PathBase}'", pathBase);
                app.UsePathBase(pathBase);
            }


            app.UseSession();
            app.UseStaticFiles();

            app.UseAuthentication();

            WebContextSeed.Seed(app, env, loggerFactory);

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Catalog}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "defaultError",
                    template: "{controller=Error}/{action=Error}");
            });
        }
    }

    static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppSettings>(configuration);

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession();

            if (configuration.GetValue<string>("IsClusterEnv") == bool.TrueString)
            {
                services.AddDataProtection(opts =>
                {
                    opts.ApplicationDiscriminator = "eshop.webmvc";
                })
                .PersistKeysToRedis(ConnectionMultiplexer.Connect(configuration["DPConnectionString"]), "DataProtection-Keys");
            }
            return services;
        }

        // Adds all Http client services (like Service-Agents) using resilient Http requests based on HttpClient factory and Polly's policies
        public static IServiceCollection AddHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //register delegating handlers
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
            services.AddTransient<HttpClientRequestIdDelegatingHandler>();

            //set 5 min as the lifetime for each HttpMessageHandler int the pool
            services.AddHttpClient("extendedhandlerlifetime").SetHandlerLifetime(TimeSpan.FromMinutes(5));

            //add http client services
            services.AddHttpClient<IBasketService, BasketService>()
                   .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Sample. Default lifetime is 2 minutes
                   .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

            services.AddHttpClient<ICatalogService, CatalogService>()
                   .AddPolicyHandler(GetRetryPolicy())
                   .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

            services.AddHttpClient<IOrderingService, OrderingService>()
                 .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                 .AddHttpMessageHandler<HttpClientRequestIdDelegatingHandler>()
                 .AddPolicyHandler(GetRetryPolicy())
                 .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

            services.AddHttpClient<ICampaignService, CampaignService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AddPolicyHandler(GetRetryPolicy())
                .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

            services.AddHttpClient<ILocationService, LocationService>()
               .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
               .AddPolicyHandler(GetRetryPolicy())
               .AddPolicyHandler(GetCircuitBreakerPolicy(configuration));

            //add custom application services
            services.AddTransient<IIdentityParser<ApplicationUser>, IdentityParser>();

            return services;
        }

        public static IServiceCollection AddHttpClientLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(b =>
            {
                b.AddFilter((category, level) => true); // Spam the world with logs.

                // Add console logger so we can see all the logging produced by the client by default.
                b.AddConsole(c => c.IncludeScopes = true);

                // Add console logger
                b.AddDebug();
            });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var identityUrl = configuration.GetValue<string>("IdentityUrl");
            var callBackUrl = configuration.GetValue<string>("CallBackUrl");
            var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

            // Add Authentication services
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(setup=>setup.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = identityUrl;
                options.SignedOutRedirectUri = callBackUrl;
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("basket");
                options.Scope.Add("orders");
                options.Scope.Add("webshoppingagg");
                options.Scope.Add("orders.signalrhub");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "role",
                    RoleClaimType = "role"
                };
            });

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(IConfiguration configuration)
        {
            var secondTimeout = Int16.Parse(configuration.GetValue<string>("CircuitTimeout", "90"));
            return HttpPolicyExtensions.HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(secondTimeout));
        }
    }
}
