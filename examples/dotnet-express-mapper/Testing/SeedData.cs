using dotnet_express_mapper.Data;
using dotnet_express_mapper.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Testing
{
    public class SeedData
    {
        public void InitData(AppDbContext context)
        {
            CreateBooks(context).Wait();
        }

        private async Task CreateBooks(AppDbContext context)
        {
			await context.Authors.AddRangeAsync(new List<Author> {
				new Author { FirstName = "Stephen", LastName = "Ken" },
				new Author { FirstName = "David", LastName = "Coup" },
				new Author { FirstName = "Tom", LastName = "Hank" }
			});

			var category = new Category
            {
                CategoryName = "Network"
            };

            var category1 = new Category
            {
                CategoryName = "Programming"
            };
            await context.Categories.AddRangeAsync(new List<Category> { category, category1 });
            await context.SaveChangesAsync();

            var book = new Book
            {
                Price = (decimal)43.45,
                BookName = "Basic C#",
                AuthorId = 1
            };

            book.BookCategories = new List<BookCategory>
            {
                new BookCategory
                {
                    Category = category1,
                    Book = book
                }
            };

            //Now add this book, with all its relationships, to the database
            await context.Books.AddRangeAsync(new List<Book> { book });
			//await context.BookCategories.AddRangeAsync(book.BookCategories);
			await context.SaveChangesAsync();

        }
    }
}
