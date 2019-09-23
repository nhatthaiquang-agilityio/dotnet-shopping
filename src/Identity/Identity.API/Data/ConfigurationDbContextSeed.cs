using Identity.API.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Data
{
    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {

            //callbacks urls from config:
            var clientUrls = new Dictionary<string, string>();
            clientUrls.Add("BasketApi", configuration.GetValue<string>("BasketApiClient"));
            clientUrls.Add("Mvc", configuration.GetValue<string>("MvcClient"));
            clientUrls.Add("MapUserMvc", configuration.GetValue<string>("MapUserMvcClient"));
            clientUrls.Add("WebhooksWeb", configuration.GetValue<string>("WebhooksWebClient"));
            clientUrls.Add("WebhooksApi", configuration.GetValue<string>("WebhooksApiClient"));
            clientUrls.Add("WebShoppingAgg", configuration.GetValue<string>("WebShoppingAggClient"));
            clientUrls.Add("OrderingApi", configuration.GetValue<string>("OrderingApiClient"));

            // Clients
            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients(clientUrls))
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            // Identity Resources
            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            // Api Resources
            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.GetApis())
                {
                    context.ApiResources.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
