using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    /// <summary>
    /// Defines operations provided by the product service layer.
    /// The service is responsible for retrieving and updating product data
    /// and acts as a thin layer over the data provider to apply simple
    /// validation, paging and logging policies.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A <see cref="List{Product}"/> containing every product available.</returns>
        public Task<List<Product>> GetProductsAsync();

        /// <summary>
        /// Retrieves a page of products.
        /// </summary>
        /// <param name="pageNumber">1-based page number. Values less than 1 are typically normalized by the implementation.</param>
        /// <param name="pageSize">Number of items per page. Values less than 1 are typically normalized by the implementation.</param>
        /// <returns>A <see cref="List{Product}"/> containing the requested page of products.</returns>
        public Task<List<Product>> GetProductsAsync(int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Retrieves a single product by its identifier.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns>The <see cref="Product"/> when found; otherwise <c>null</c>.</returns>
        public Task<Product?> GetProductByIdAsync(int id);

        /// <summary>
        /// Updates the description for the specified product.
        /// </summary>
        /// <param name="id">The product identifier to update.</param>
        /// <param name="description">The new description text.</param>
        /// <returns><c>true</c> when the update succeeded (typically when a single row was affected); otherwise <c>false</c>.</returns>
        public Task<bool> UpdateProductDescriptionAsync(int id, string description);
    }
}
