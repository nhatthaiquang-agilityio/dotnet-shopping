using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityUsers.Data;
using IdentityUsers.Hubs;
using IdentityUsers.Models;
using IdentityUsers.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityUsers
{
    public class Startup
    {
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());
        private static readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionString"]));

            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 12;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddCookie(options => {
                    options.LoginPath = "/Account/Unauthorized/";
                    options.AccessDeniedPath = "/Account/Forbidden/";
                })
                .AddJwtBearer(options =>
                {
                    // Configure JWT Bearer Auth to expect our security key
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            LifetimeValidator = (before, expires, token, param) =>
                            {
                                return expires > DateTime.UtcNow;
                            },
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateActor = false,
                            ValidateLifetime = true,
                            IssuerSigningKey = SecurityKey
                        };

                    // We have to hook the OnMessageReceived event in order to
                    // allow the JWT authentication handler to read the access
                    // token from the query string when a WebSocket or
                    // Server-Sent Events request comes in.
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/ChatHub")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsAdminClaimAccess", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("role", "admin");
                });

            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // using azure signal R
            if (Configuration.GetValue<bool>("UseAzureSignalR"))
                services.AddSignalR().AddAzureSignalR();
            else
                services.AddSignalR();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account");
                });

            // https://dotnetcoretutorials.com/2018/03/20/cannot-consume-scoped-service-from-singleton-a-lesson-in-asp-net-core-di-scopes/
            // only use addScoped
            services.AddScoped<IUserConnectionManager, UserConnectionManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            InitUserClaims(app);

            app.UseMvc();

            if (Configuration.GetValue<bool>("UseAzureSignalR"))
                app.UseAzureSignalR(routes =>
                {
                    routes.MapHub<ChatHub>("/ChatHub");
                });
            else
                app.UseSignalR(hubs =>
                {
                    hubs.MapHub<ChatHub>("/ChatHub");
                });
        }

        internal static class Claims
        {
            public static List<Claim> Get()
            {
                return new List<Claim> {
                    new Claim("role", "admin")
                };
            }
        }

        private static IEnumerable<ApplicationUser> GetDefaultUser()
        {
            var user = new ApplicationUser
            {

                Email = "nhatthai@gmail.com",
                Id = Guid.NewGuid().ToString(),
                LastName = "Thai",
                PhoneNumber = "1234567890",
                UserName = "nhatthai@gmail.com",
                NormalizedEmail = "NHATTHAI@GMAIL.COM",
                NormalizedUserName = "NHATTHAI@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");

            var admin = new ApplicationUser
            {
                Email = "admin@gmail.com",
                Id = Guid.NewGuid().ToString(),
                LastName = "Admin",
                PhoneNumber = "1234567890",
                UserName = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            admin.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");
            return new List<ApplicationUser>
            {
                user, admin
            };
        }

        private static void InitUserClaims(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // add users
                foreach (var user in GetDefaultUser())
                {
                    userManager.CreateAsync(user).Wait();
                }

                if (userManager.Users.Any())
                {
                    foreach (var user in userManager.Users)
                    {
                        // add claim and role for admin
                        if (user.UserName.Equals("admin@gmail.com"))
                        {
                            // add claim: admin
                            userManager.AddClaimsAsync(user, Claims.Get()).Wait();

                            // add role: Administrator
                            //userManager.AddToRoleAsync(user, "Administrator").Wait();
                        }
                    }
                }
            }
        }
    }
}
