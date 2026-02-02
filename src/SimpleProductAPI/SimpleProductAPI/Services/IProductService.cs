
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    public interface IProductService
    {
        public Task<List<Product>> GetProducts();
        public Task<Product> GetProductsById(int id);
        public Task<bool> UpdateProductDescription(int id, string description);
    }
}
