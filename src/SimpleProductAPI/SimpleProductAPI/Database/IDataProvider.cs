using SimpleProductAPI.Models;

namespace SimpleProductAPI.Data
{
    public interface IDataProvider
    {
        Task<Product?> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<bool> UpdateProductDescription(int id, string description);
    }
}
