using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    public class ProductService : IProductService
    {
        public Task<List<Product>> GetProducts()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetProductsById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateProductDescription(int id, string description)
        {
            throw new NotImplementedException();
        }
    }
}
