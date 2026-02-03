using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleProductAPI.Data;
using SimpleProductAPI.Models;
using SimpleProductAPI.Services;
using Xunit;

namespace SimpleProductAPI.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IDataProvider> _dataProviderMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _dataProviderMock = new Mock<IDataProvider>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_dataProviderMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var list = new List<Product>
            {
                new Product { Id = 1, Name = "A", ImageUri = "i1", Price = 1m },
                new Product { Id = 2, Name = "B", ImageUri = "i2", Price = 2m }
            };
            _dataProviderMock.Setup(d => d.GetProductsAsync()).ReturnsAsync(list);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            _dataProviderMock.Verify(d => d.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_WithPaging_ReturnsCorrectPage()
        {
            // Arrange
            var list = Enumerable.Range(1, 5)
                .Select(i => new Product { Id = i, Name = $"P{i}", ImageUri = $"img{i}", Price = i }).ToList();
            _dataProviderMock.Setup(d => d.GetProductsAsync()).ReturnsAsync(list);

            // Act: page 2, size 2 -> items with Id 3 and 4
            var result = await _service.GetProductsAsync(2, 2);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(3, result[0].Id);
            Assert.Equal(4, result[1].Id);
            _dataProviderMock.Verify(d => d.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductsAsync_NegativePageOrSize_NormalizesToOne()
        {
            // Arrange
            var list = Enumerable.Range(1, 3)
                .Select(i => new Product { Id = i, Name = $"P{i}", ImageUri = $"img{i}", Price = i }).ToList();
            _dataProviderMock.Setup(d => d.GetProductsAsync()).ReturnsAsync(list);

            // Act
            var result = await _service.GetProductsAsync(-10, 0);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            _dataProviderMock.Verify(d => d.GetProductsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductExists_ReturnsProduct()
        {
            // Arrange
            var product = new Product { Id = 7, Name = "Exists", ImageUri = "img", Price = 7m };
            _dataProviderMock.Setup(d => d.GetProductByIdAsync(7)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductByIdAsync(7);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result!.Id);
            _dataProviderMock.Verify(d => d.GetProductByIdAsync(7), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ProductMissing_ReturnsNull()
        {
            // Arrange
            _dataProviderMock.Setup(d => d.GetProductByIdAsync(99)).ReturnsAsync((Product)null);

            // Act
            var result = await _service.GetProductByIdAsync(99);

            // Assert
            Assert.Null(result);
            _dataProviderMock.Verify(d => d.GetProductByIdAsync(99), Times.Once);
        }

        [Theory]
        [InlineData(1, "new desc", true)]
        [InlineData(2, "other", false)]
        public async Task UpdateProductDescriptionAsync_DelegatesToDataProvider_ReturnsFlag(int id, string desc, bool expected)
        {
            // Arrange
            _dataProviderMock.Setup(d => d.UpdateProductDescription(id, desc)).ReturnsAsync(expected);

            // Act
            var result = await _service.UpdateProductDescriptionAsync(id, desc);

            // Assert
            Assert.Equal(expected, result);
            _dataProviderMock.Verify(d => d.UpdateProductDescription(id, desc), Times.Once);
        }
    }
}