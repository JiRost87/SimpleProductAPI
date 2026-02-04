using SimpleProductAPI.Data;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Services
{
    /// <summary>
    /// Service layer that provides product-related operations.
    /// Delegates data access to an <see cref="IDataProvider"/> and performs
    /// lightweight validation, paging, and logging.
    /// </summary>
    public class ProductService : IProductService
    {
        private const int MAX_PAGE_SIZE = 100;

        // Underlying data provider used to fetch and update product data.
        private readonly IDataProvider _dataProvider;

        // Logger for diagnostic and tracing information.
        private readonly ILogger<ProductService> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ProductService"/>.
        /// </summary>
        /// <param name="dataProvider">Data provider implementation for persistence operations.</param>
        /// <param name="logger">Logger instance for this service.</param>
        public ProductService(IDataProvider dataProvider, ILogger<ProductService> logger) 
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <returns>A list containing every product returned by the data provider.</returns>
        public async Task<List<Product>> GetProductsAsync()
        {
            _logger.LogDebug("GetProductsAsync: retrieving all products");

            // Request all products from the data provider.
            var products = await _dataProvider.GetProductsAsync();

            // Materialize to a List for callers that expect random access / Count.
            var list = products.ToList();

            _logger.LogDebug("GetProductsAsync: retrieved {Count} products", list.Count);
            return list;
        }

        /// <summary>
        /// Retrieves a single page of products using in-memory paging.
        /// Performs basic validation on page parameters.
        /// </summary>
        /// <param name="pageNumber">1-based page number; values less than 1 are normalized to 1.</param>
        /// <param name="pageSize">Number of items per page; values less than 1 are normalized to 1.</param>
        /// <returns>A list containing the requested page of products.</returns>
        public async Task<List<Product>> GetProductsAsync(int pageNumber, int pageSize)
        {
            _logger.LogDebug("GetProductsAsync(paging): page={Page}, size={Size}", pageNumber, pageSize);

            // Basic validation for paging parameters.
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 1;
            if (pageSize > MAX_PAGE_SIZE)
            {
                _logger.LogWarning("GetProductsAsync(paging): pageSize {Size} exceeds max, capping to {MaxSize}", pageSize, MAX_PAGE_SIZE);
                pageSize = MAX_PAGE_SIZE;
            }

            // Fetch all products then apply in-memory paging.
            var products = await _dataProvider.GetProductsAsync();
            var pagedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogDebug("GetProductsAsync(paging): returning {Count} items", pagedProducts.Count);
            return pagedProducts;
        }

        /// <summary>
        /// Retrieves a product by its identifier.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>The product if found; otherwise null.</returns>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("GetProductByIdAsync: id={Id}", id);

            // Delegate to data provider. Log when not found for diagnostics.
            var product = await _dataProvider.GetProductByIdAsync(id);
            if (product == null)
                _logger.LogDebug("GetProductByIdAsync: not found id={Id}", id);

            return product;
        }

        /// <summary>
        /// Updates the description of an existing product.
        /// </summary>
        /// <param name="id">Product identifier to update.</param>
        /// <param name="description">New description text.</param>
        /// <returns>True when the update affected exactly one record; otherwise false.</returns>
        public async Task<bool> UpdateProductDescriptionAsync(int id, string description)
        {
            _logger.LogDebug("UpdateProductDescriptionAsync: id={Id}", id);

            // Delegate update to the data provider and log the result.
            var result = await _dataProvider.UpdateProductDescription(id, description);
            _logger.LogDebug("UpdateProductDescriptionAsync: result={Result} for id={Id}", result, id);

            return result;
        }
    }
}
