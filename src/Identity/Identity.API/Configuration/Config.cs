using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.API.Configuration
{
    public class Config
    {
        // ApiResources define the apis in your system
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                //new ApiResource("orders", "Orders Service"),
                new ApiResource("basket", "Basket API Service"),
                //new ApiResource("marketing", "Marketing Service"),
                //new ApiResource("locations", "Locations Service"),
                //new ApiResource("mobileshoppingagg", "Mobile Shopping Aggregator"),
                //new ApiResource("webshoppingagg", "Web Shopping Aggregator"),
                //new ApiResource("orders.signalrhub", "Ordering Signalr Hub"),
                //new ApiResource("webhooks", "Webhooks registration Service"),
            };
        }

        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["BasketApi"]}/oauth2-redirect.html" },
                    //PostLogoutRedirectUris = { $"{clientsUrl["BasketApi"]}/" },

                    AllowedScopes =
                    {
                        "basket"
                    }
                }

            };
        }
    }
}