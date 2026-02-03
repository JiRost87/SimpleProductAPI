
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();
        public Task<Product> GetProductById(int id);
        public Task<bool> UpdateProductDescription(int id, string description);
    }
}
