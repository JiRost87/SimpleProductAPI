using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleProductAPI.Controllers.v2;
using SimpleProductAPI.Models;
using SimpleProductAPI.Services;
using Xunit;

namespace SimpleProductAPI.Tests.Controllers.v2
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductController>>();
            _controller = new ProductController(_productServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetProductsAsync_DefaultParameters_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "A", ImageUri = "i", Price = 1m } };
            _productServiceMock.Setup(s => s.GetProductsAsync(1, 10)).ReturnsAsync(products);

            // Act
            var actionResult = await _controller.GetProductsAsync();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsType<List<Product>>(ok.Value);
            Assert.Single(returned);
            _productServiceMock.Verify(s => s.GetProductsAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetProductAsync_ProductExists_ReturnsOkWithProduct()
        {
            // Arrange
            var product = new Product { Id = 7, Name = "P", ImageUri = "img", Price = 7m };
            _productServiceMock.Setup(s => s.GetProductByIdAsync(7)).ReturnsAsync(product);

            // Act
            var actionResult = await _controller.GetProductAsync(7);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returned = Assert.IsType<Product>(ok.Value);
            Assert.Equal(7, returned.Id);
        }

        [Fact]
        public async Task GetProductAsync_ProductMissing_ReturnsNotFound()
        {
            // Arrange
            _productServiceMock.Setup(s => s.GetProductByIdAsync(99)).ReturnsAsync((Product)null);

            // Act
            var actionResult = await _controller.GetProductAsync(99);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task UpdateProductDescriptionAsync_NullDescription_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UpdateProductDescriptionAsync(1, null!);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Description is required", badRequest.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdateProductDescriptionAsync_EmptyOrWhitespaceDescription_ReturnsBadRequest(string desc)
        {
            // Arrange
            var dto = new UpdateDescriptionDto { Description = desc };

            // Act
            var result = await _controller.UpdateProductDescriptionAsync(1, dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Description is required", badRequest.Value);
        }

        [Fact]
        public async Task UpdateProductDescriptionAsync_ServiceReturnsFalse_ReturnsNotFound()
        {
            // Arrange
            var dto = new UpdateDescriptionDto { Description = "new" };
            _productServiceMock.Setup(s => s.UpdateProductDescriptionAsync(1, "new")).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateProductDescriptionAsync(1, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateProductDescriptionAsync_ServiceReturnsTrue_ReturnsNoContent()
        {
            // Arrange
            var dto = new UpdateDescriptionDto { Description = "updated" };
            _productServiceMock.Setup(s => s.UpdateProductDescriptionAsync(2, "updated")).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateProductDescriptionAsync(2, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Constructor guard tests
        [Fact]
        public void Ctor_NullService_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<ProductController>>();
            Assert.Throws<ArgumentNullException>(() => new ProductController(null!, logger.Object));
        }

        [Fact]
        public void Ctor_NullLogger_ThrowsArgumentNullException()
        {
            var service = new Mock<IProductService>();
            Assert.Throws<ArgumentNullException>(() => new ProductController(service.Object, null!));
        }
    }
}