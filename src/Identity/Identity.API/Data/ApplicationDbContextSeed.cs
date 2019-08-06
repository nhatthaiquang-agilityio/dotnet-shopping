using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Identity.API.Extensions;
using Identity.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using IdentityModel;

namespace Identity.API.Data
{
    public class ApplicationDbContextSeed
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = new PasswordHasher<ApplicationUser>();

        public async Task SeedAsync(ApplicationDbContext context,IHostingEnvironment env,
            ILogger<ApplicationDbContextSeed> logger, IOptions<AppSettings> settings,int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                var useCustomizationData = settings.Value.UseCustomizationData;
                var contentRootPath = env.ContentRootPath;
                var webroot = env.WebRootPath;

                if (!context.Users.Any())
                {
                    context.Users.AddRange(useCustomizationData
                        ? GetUsersFromFile(contentRootPath, logger)
                        : GetDefaultUser());
                    await context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    logger.LogError(ex, "EXCEPTION ERROR while migrating {DbContextName}", nameof(ApplicationDbContext));

                    await SeedAsync(context,env,logger,settings, retryForAvaiability);
                }
            }
        }

        private IEnumerable<ApplicationUser> GetUsersFromFile(string contentRootPath, ILogger logger)
        {
            string csvFileUsers = Path.Combine(contentRootPath, "Setup", "Users.csv");

            if (!File.Exists(csvFileUsers))
            {
                return GetDefaultUser();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = {
                    "cardholdername", "cardnumber", "cardtype", "city", "country",
                    "email", "expiration", "lastname", "name", "phonenumber",
                    "username", "zipcode", "state", "street", "securitynumber",
                    "normalizedemail", "normalizedusername", "password"
                };
                csvheaders = GetHeaders(requiredHeaders, csvFileUsers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);

                return GetDefaultUser();
            }

            List<ApplicationUser> users = File.ReadAllLines(csvFileUsers)
                .Skip(1) // skip header column
                .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)") )
                .SelectTry(column => CreateApplicationUser(column, csvheaders))
                .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                .Where(x => x != null)
                .ToList();

            return users;
        }

        private ApplicationUser CreateApplicationUser(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            string cardtypeString = column[Array.IndexOf(headers, "cardtype")].Trim('"').Trim();
            if (!int.TryParse(cardtypeString, out int cardtype))
            {
                throw new Exception($"cardtype='{cardtypeString}' is not a number");
            }

            var user = new ApplicationUser
            {
                CardHolderName = column[Array.IndexOf(headers, "cardholdername")].Trim('"').Trim(),
                CardNumber = column[Array.IndexOf(headers, "cardnumber")].Trim('"').Trim(),
                CardType = cardtype,
                City = column[Array.IndexOf(headers, "city")].Trim('"').Trim(),
                Country = column[Array.IndexOf(headers, "country")].Trim('"').Trim(),
                Email = column[Array.IndexOf(headers, "email")].Trim('"').Trim(),
                Expiration = column[Array.IndexOf(headers, "expiration")].Trim('"').Trim(),
                Id = Guid.NewGuid().ToString(),
                LastName = column[Array.IndexOf(headers, "lastname")].Trim('"').Trim(),
                Name = column[Array.IndexOf(headers, "name")].Trim('"').Trim(),
                PhoneNumber = column[Array.IndexOf(headers, "phonenumber")].Trim('"').Trim(),
                UserName = column[Array.IndexOf(headers, "username")].Trim('"').Trim(),
                ZipCode = column[Array.IndexOf(headers, "zipcode")].Trim('"').Trim(),
                State = column[Array.IndexOf(headers, "state")].Trim('"').Trim(),
                Street = column[Array.IndexOf(headers, "street")].Trim('"').Trim(),
                SecurityNumber = column[Array.IndexOf(headers, "securitynumber")].Trim('"').Trim(),
                NormalizedEmail = column[Array.IndexOf(headers, "normalizedemail")].Trim('"').Trim(),
                NormalizedUserName = column[Array.IndexOf(headers, "normalizedusername")].Trim('"').Trim(),
                SecurityStamp = Guid.NewGuid().ToString("D"),
                PasswordHash = column[Array.IndexOf(headers, "password")].Trim('"').Trim()
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            return user;
        }

        private IEnumerable<ApplicationUser> GetDefaultUser()
        {
            var user = new ApplicationUser
            {
                CardHolderName = "Nhat Thai",
                CardNumber = "4012888888881881",
                CardType = 1,
                City = "Da Nang",
                Country = "VN",
                Email = "nhatthai@gmail.com",
                Expiration = "12/20",
                Id = Guid.NewGuid().ToString(),
                LastName = "Thai",
                Name = "NhatThai",
                PhoneNumber = "1234567890",
                UserName = "nhatthai@gmail.com",
                ZipCode = "98052",
                State = "DN",
                Street = "697 Thanh Thuy",
                SecurityNumber = "535",
                NormalizedEmail = "NHATTHAI@GMAIL.COM",
                NormalizedUserName = "NHATTHAI@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");

            var admin = new ApplicationUser
            {
                CardHolderName = "Admin",
                CardNumber = "4012888888881333",
                CardType = 1,
                City = "Da Nang",
                Country = "VN",
                Email = "admin@gmail.com",
                Expiration = "12/20",
                Id = Guid.NewGuid().ToString(),
                LastName = "Admin",
                Name = "Admin",
                PhoneNumber = "1234567890",
                UserName = "admin@gmail.com",
                ZipCode = "98052",
                State = "DN",
                Street = "69 Hoang Dieu",
                SecurityNumber = "544",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                NormalizedUserName = "ADMIN@GMAIL.COM",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            admin.PasswordHash = _passwordHasher.HashPassword(user, "Pass@word1");
            return new List<ApplicationUser>()
            {
                user, admin
            };
        }

        static string[] GetHeaders(string[] requiredHeaders, string csvfile)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() != requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is different then read header '{csvheaders.Count()}'");
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }

        private static class Claims
        {
            public static List<Claim> Get()
            {
                return new List<Claim> {
                    new Claim(JwtClaimTypes.Role, "api.admin")
                };
            }
        }

        private static class Roles
        {
            public static List<string> Get()
            {
                return new List<string> { "Admin", "Manager", "User" };
            }
        }

        // seed user claims data
        public void InitUserClaims(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetService<IServiceScopeFactory>().CreateScope())
            {
                // claim
                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                if (userManager.Users.Any())
                {
                    foreach (var user in userManager.Users)
                    {
                        if (user.UserName.Equals("admin@gmail.com"))
                        {
                            // Using in basket service
                            // Apply policy claim and role for Authorize
                            // add claim: admin
                            userManager.AddClaimsAsync(user, Claims.Get()).Wait();

                            // add role: Admin
                            userManager.AddToRoleAsync(user, "Admin").Wait();
                        }
                        else
                        {
                            // add role: User
                            userManager.AddToRoleAsync(user, "User").Wait();
                        }
                    }
                }
            }
        }

        // Create User Roles
        public async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in Roles.Get())
            {
                //Adding Addmin Role
                var roleCheck = await roleManager.RoleExistsAsync(roleName);
                if (!roleCheck)
                {
                    //create the roles and seed them to the database
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
