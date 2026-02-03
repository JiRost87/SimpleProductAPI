
namespace SimpleProductAPI.Models
{
    public class Product
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string ImageUri {get ; set; }
        public required decimal Price { get; set; }
        public string? Description { get; set; }

    }
}
