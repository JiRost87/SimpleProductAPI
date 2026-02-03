using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Controllers.v2
{
    [ApiVersion("2")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        Product product = new Product { Id = 1, Name = "Test", ImageUri = new Uri("http://test.html"), Price = 10.5M };

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProductsAsync() { 
            var products = new List<Product>();
            products.Add(product);
            return (Ok(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductAsync(int id) {
            return (Ok(product));
        }

        [HttpPut("{id}/{description}")]
        public async Task<ActionResult> UpdateProductDescriptionAsync(int id, string description)
        {
            return Ok();
        }
    }
}
