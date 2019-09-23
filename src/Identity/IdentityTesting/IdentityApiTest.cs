using System.Net.Http;
using System.Threading.Tasks;
using Identity.API;
using Xunit;

namespace IdentityTesting
{
    public class IdentityApiTest : IClassFixture<CustomWebAppFactory<Startup>>
    {
        private readonly HttpClient _client;
        public IdentityApiTest(CustomWebAppFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task NoPermission()
        {
            // Act
            var response = await _client.GetAsync("/api/identity");

            // Assert
            response.StatusCode.Equals(400);
        }

        [Fact]
        public async Task NoRequiredIdentity()
        {
            // Act
            var response = await _client.GetAsync("/api/identity/no");

            // Assert
            response.EnsureSuccessStatusCode();
        }

    }
}
