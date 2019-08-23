namespace dotnet_express_mapper.Models
{
    public class Size
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
