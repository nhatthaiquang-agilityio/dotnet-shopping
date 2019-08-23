using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet_express_mapper.Data;
using dotnet_express_mapper.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_express_mapper.Services
{
    public class BookService
    {
        private readonly AppDbContext _appDBContext;
        public BookService(AppDbContext context)
        {
            _appDBContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Book>> GetBooks()
        {
            return await _appDBContext.Books.Include(a => a.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(c => c.Category)
                .ToListAsync();
        }

        public async Task<Book> GetBook(int id)
        {
            return await _appDBContext.Books.Include(a => a.Author)
                .Include(c => c.BookCategories)
                    .ThenInclude(c => c.Category)
                .SingleAsync(b => b.Id == id);
        }

        public async Task Create(Book book)
        {
            await _appDBContext.Books.AddAsync(book);
            await _appDBContext.SaveChangesAsync();
        }

        public async Task<bool> Delete(int id)
        {
            Book book = await _appDBContext.Books.SingleAsync(b => b.Id == id);
            _appDBContext.Books.Remove(book);
            await _appDBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Update(Book bookIn)
        {
            _appDBContext.Books.Update(bookIn);
            await _appDBContext.SaveChangesAsync();
            return true;
        }
    }
}
