namespace dotnet_express_mapper.Models
{
    public class BookCategoryViewModel
    {
        public int BookId { get; set; }
        public int CategoryId { get; set; }
        public CategoryViewModel Category { get; set; }
    }
}