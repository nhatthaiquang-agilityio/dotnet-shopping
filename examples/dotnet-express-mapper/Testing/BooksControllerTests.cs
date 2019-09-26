using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using dotnet_express_mapper;
using dotnet_express_mapper.Models;
using Newtonsoft.Json;
using Xunit;

namespace Testing
{
    // Integration Test
    public class BooksControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly string API = "http://localhost/api/books/";

        public BooksControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task TestGetListBooks()
        {
            var httpResponse = await _client.GetAsync(API);

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<IEnumerable<BookViewModel>>(stringResponse);

            Assert.Contains(books, p => p.Author == "Stephen Ken");
            Assert.Contains(books, p => p.BookName == "Basic C#");
        }

        [Fact]
        public async Task TestCreateBook()
        {
            var bookModel = new BookViewModel
            {
                AuthorId = 2,
                Price = (decimal)43.45,
                BookName = "Advance C#"
            };

            HttpContent content = new StringContent(
                JsonConvert.SerializeObject(bookModel),
                Encoding.UTF8, "application/json");
            var httpResponse = await _client.PostAsync(API, content);

            httpResponse.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            var stringResponse = await httpResponse.Content.ReadAsStringAsync();
            var book = JsonConvert.DeserializeObject<Book>(stringResponse);

            Assert.Equal(43.45, (double)book.Price);
            Assert.Equal("Advance C#", book.BookName);
            Assert.True(book.Id > 0);
        }

        [Fact]
        public async Task TestGetBook()
        {
            var httpResponse = await _client.GetAsync(API + "1");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();

            // Deserialize results.
            var contentResponse = await httpResponse.Content.ReadAsStringAsync();
            var bookObj = JsonConvert.DeserializeObject<BookViewModel>(contentResponse);

            Assert.Equal(43.45, (double)bookObj.Price);
            Assert.Contains("Basic C#", bookObj.BookName);
        }
    }
}
