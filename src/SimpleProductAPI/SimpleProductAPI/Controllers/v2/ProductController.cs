using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SimpleProductAPI.Models;
using SimpleProductAPI.Services;

namespace SimpleProductAPI.Controllers.v2
{
    /// <summary>
    /// API controller for v2 product operations.
    /// Exposes endpoints to retrieve products and update product descriptions.
    /// Versioning is applied via URL segment (v{version}).
    /// </summary>
    [ApiVersion("2")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private const int MAX_PAGE_SIZE = 100;
        // Service used to access product data and operations.
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ProductController"/>.
        /// </summary>
        /// <param name="productService">Injected product service used to perform product operations.</param>
        /// <param name="logger">Injected logger.</param>
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all products with pagination.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>HTTP 200 with a list of <see cref="Product"/> when successful.</returns>
        [HttpGet("GetProducts")]
        public async Task<ActionResult<List<Product>>> GetProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("v2 GetProductsAsync called page={Page} size={Size}", pageNumber, pageSize);
            var products = await _productService.GetProductsAsync(pageNumber, pageSize);
            _logger.LogInformation("v2 GetProductsAsync returned {Count} products", products?.Count ?? 0);
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a single product by id.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>
        /// HTTP 200 (OK) with the <see cref="Product"/> when found 
        /// HTTP 404 (Not Found) for missing products.
        /// </returns>
        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<Product>> GetProductAsync(int id)
        {
            _logger.LogInformation("v2 GetProductAsync called for id={Id}", id);
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("v2 GetProductAsync: not found id={Id}", id);
                return NotFound();
            }
            _logger.LogInformation("v2 GetProductAsync: found id={Id}", id);
            return Ok(product);
        }

        /// <summary>
        /// Updates the description of an existing product.
        /// </summary>
        /// <param name="id">Product identifier to update.</param>
        /// <param name="dto">DTO containing the new description. Must not be null or whitespace.</param>
        /// <returns>
        /// HTTP 204 (No Content) when update succeeds;
        /// HTTP 400 (Bad Request) when <paramref name="dto"/> is null or description is empty;
        /// HTTP 404 (Not Found) when the product does not exist.
        /// </returns>
        [HttpPut("UpdateProductDescription/{id}")]
        public async Task<ActionResult> UpdateProductDescriptionAsync(int id, [FromBody] UpdateDescriptionDto dto)
        {
            _logger.LogInformation("v2 UpdateProductDescriptionAsync called for id={Id}", id);

            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                _logger.LogWarning("v2 UpdateProductDescriptionAsync: invalid payload for id={Id}", id);
                return BadRequest("Description is required");
            }

            var result = await _productService.UpdateProductDescriptionAsync(id, dto.Description);
            if (!result)
            {
                _logger.LogWarning("v2 UpdateProductDescriptionAsync: product not found for id={Id}", id);
                return NotFound();
            }

            _logger.LogInformation("v2 UpdateProductDescriptionAsync: updated id={Id}", id);
            return NoContent();
        }
    }
}
