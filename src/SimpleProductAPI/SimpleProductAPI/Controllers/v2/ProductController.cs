using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SimpleProductAPI.Models;
using SimpleProductAPI.Services;

namespace SimpleProductAPI.Controllers.v2
{
    [ApiVersion("2")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProductsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 1;
            pageSize = Math.Min(pageSize, 500);

            var products = await _productService.GetProductsAsync(pageNumber, pageSize, cancellationToken);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        public class UpdateDescriptionDto { public string Description { get; set; } }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProductDescriptionAsync(int id, [FromBody] UpdateDescriptionDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Description))
            {
                return BadRequest("Description is required");
            }

            var result = await _productService.UpdateProductDescriptionAsync(id, dto.Description, cancellationToken);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
