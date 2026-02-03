
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    public interface IProductService
    {
        public Task<List<Product>> GetProductsAsync(int pageNumber = 1, int pageSize = 10);
        public Task<Product?> GetProductByIdAsync(int id);
        public Task<bool> UpdateProductDescriptionAsync(int id, string description);
    }
}
