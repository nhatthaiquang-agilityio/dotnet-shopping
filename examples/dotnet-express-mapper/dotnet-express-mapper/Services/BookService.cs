using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_express_mapper.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace dotnet_express_mapper.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IConfiguration config)
        {
            string connectionString = (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MONGO_HOST"))) ?
                config.GetConnectionString("BookstoreDb") : config["MongoDB:Container"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(config["MongoDB:Database"]);
            _books = database.GetCollection<Book>("Books");
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _books.Find(Book => true).ToListAsync();
        }

        public async Task<Book> GetBook(string id)
        {
            return await _books.Find(Book => Book.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Book book)
        {
            await _books.InsertOneAsync(book);
        }

        public async Task<bool> Delete(string id)
        {
            DeleteResult deleteResult = await _books.DeleteOneAsync(book => book.Id == id);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<bool> Update(Book bookIn)
        {
            ReplaceOneResult updateResult =
                await _books.ReplaceOneAsync(book => book.Id == bookIn.Id, bookIn);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }
    }
}
