using System.ComponentModel.DataAnnotations;

namespace dotnet_express_mapper.Models
{
    public class ProductType
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }
    }
}