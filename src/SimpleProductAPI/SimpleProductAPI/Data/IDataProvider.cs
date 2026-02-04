using SimpleProductAPI.Models;

namespace SimpleProductAPI.Data
{
    /// <summary>
    /// Defines operations provided by the product db layer.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Retrieves a single <see cref="Product"/> by identifier.
        /// Returns <c>null</c> when no product is found.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>The product or <c>null</c> if not found.</returns>
        Task<Product?> GetProductByIdAsync(int id);
        
        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Product}"/> containing all products.</returns>
        Task<IEnumerable<Product>> GetProductsAsync();

        /// <summary>
        /// Inserts or updates the product description for the specified product id.
        /// </summary>
        /// <param name="id">Product identifier to update.</param>
        /// <param name="description">New product description.</param>
        /// <returns><c>true</c> when the update affected exactly one row otherwise <c>false</c>.</returns>
        Task<bool> UpdateProductDescription(int id, string description);
    }
}
