using System;
using Identity.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityTesting
{
    public class CustomWebAppFactory<TStartup> : WebApplicationFactory<Identity.API.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Add database name for testing
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context (AppDbContext) using an in-memory database for testing.
                services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryAppDb");
                        options.UseInternalServiceProvider(serviceProvider);
                    });


                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database contexts
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<ApplicationDbContext>();
                    var env = scopedServices.GetService<IHostingEnvironment>();
                    var logger = scopedServices.GetService<ILogger<ApplicationDbContextSeed>>();
                    var settings = scopedServices.GetService<IOptions<Identity.API.AppSettings>>();
                    var contextSeed = new ApplicationDbContextSeed();

                    context.Database.EnsureCreated();

                    try
                    {
                        contextSeed.SeedAsync(context, env, logger, settings).Wait();

                        // create roles and set role into users
                        contextSeed.CreateUserRoles(serviceProvider).Wait();

                        // Create claims
                        contextSeed.InitUserClaims(serviceProvider).Wait();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the " +
                                            "database with test messages. Error: {ex.Message}");
                    }
                }
            });

        }

    }
}