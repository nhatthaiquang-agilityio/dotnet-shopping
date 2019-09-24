using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IdentityTesting
{
    public class IdentityTest : IntegrationTestBase
    {

        //[Fact]
        //public async Task GetIdentity()
        //{
        //    // Arrange
        //    await SetUpTokenFor("admin", "adminPassword");
        //    var res = await HttpClient.GetAsync("/api/identity");
        //    Console.WriteLine(res);
        //    // Assert
        //    res.EnsureSuccessStatusCode();
        //}

        [Fact]
        public async Task UserGetIdentity()
        {
            // Arrange
            await SetUpTokenFor("user", "password");
            var res = await HttpClient.GetAsync("/api/identity");
          
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}