namespace dotnet_express_mapper.Models
{
    public class BookViewModel
    {
        public string Id;
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
    }
}
