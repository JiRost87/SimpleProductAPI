using SimpleProductAPI.Data;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    /// <summary>
    /// The product service
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IDataProvider _dataProvider;

        public ProductService(IDataProvider dataProvider) 
        {
            _dataProvider = dataProvider;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _dataProvider.GetProductsAsync();
            return products.ToList();
        }

        public async Task<List<Product>> GetProductsAsync(int pageNumber, int pageSize)
        {
            // Basic validation
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 1;

            var products = await _dataProvider.GetProductsAsync();
            var pagedProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return pagedProducts;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var product = await _dataProvider.GetProductByIdAsync(id);
            return product;
        }

        public async Task<bool> UpdateProductDescriptionAsync(int id, string description)
        {
            var result = await _dataProvider.UpdateProductDescription(id, description);
            return result;
        }
    }
}
