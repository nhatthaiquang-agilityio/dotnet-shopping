using System.Collections.Generic;

namespace dotnet_express_mapper.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public decimal Price { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }

        public int AuthorId {get; set;}
        public Author Author { get; set; }
    }
}
