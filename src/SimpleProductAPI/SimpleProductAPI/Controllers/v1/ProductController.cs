using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using SimpleProductAPI.Models;
using SimpleProductAPI.Services;

namespace SimpleProductAPI.Controllers.v1
{
    [ApiVersion("1")]
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
        public async Task<ActionResult<List<Product>>> GetProductsAsync() 
        { 
            var products = await _productService.GetProductsAsync();
            return (Ok(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductAsync(int id) 
        {
            var product = await _productService.GetProductByIdAsync(id);
            return (Ok(product));
        }

        [HttpPut("{id}/{description}")]
        public async Task<ActionResult> UpdateProductDescriptionAsync(int id, string description)
        {
            var result = await _productService.UpdateProductDescriptionAsync(id, description);
            if (!result)
            {
               return NotFound();
            }
            return Ok();
        }
    }
}
