using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleProductAPI.Data;
using SimpleProductAPI.Database;
using Xunit;

namespace SimpleProductAPI.Tests.Data
{
    public class SqlDataProviderTests
    {
        private readonly Mock<IDbConnectionFactory> _dbConnectionFactoryMock;
        private readonly Mock<ILogger<SqlDataProvider>> _loggerMock;

        public SqlDataProviderTests()
        {
            _dbConnectionFactoryMock = new Mock<IDbConnectionFactory>();
            _loggerMock = new Mock<ILogger<SqlDataProvider>>();
        }

        [Fact]
        public async Task GetProductByIdAsync_WhenCreateConnectionThrows_Rethrows()
        {
            // Arrange
            var ex = new Exception("connection factory failure");
            _dbConnectionFactoryMock.Setup(f => f.CreateConnectionAsync()).ThrowsAsync(ex);

            var provider = new SqlDataProvider(_dbConnectionFactoryMock.Object, _loggerMock.Object);

            // Act & Assert
            var thrown = await Assert.ThrowsAsync<Exception>(() => provider.GetProductByIdAsync(1));

            Assert.Same(ex, thrown);
        }

        [Fact]
        public async Task GetProductsAsync_WhenConnectionCreateCommandThrows_Rethrows()
        {
            // Arrange
            var connMock = new Mock<IDbConnection>();
            connMock.Setup(c => c.CreateCommand()).Throws(new InvalidOperationException("create command failed"));

            _dbConnectionFactoryMock.Setup(f => f.CreateConnectionAsync()).ReturnsAsync(connMock.Object);

            var provider = new SqlDataProvider(_dbConnectionFactoryMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetProductsAsync());
        }

        [Fact]
        public async Task UpdateProductDescription_WhenDbCommandExecutionThrows_Rethrows()
        {
            // Arrange
            var connMock = new Mock<IDbConnection>();
            connMock.Setup(c => c.CreateCommand()).Throws(new InvalidOperationException("create command failed"));

            _dbConnectionFactoryMock.Setup(f => f.CreateConnectionAsync()).ReturnsAsync(connMock.Object);

            var provider = new SqlDataProvider(_dbConnectionFactoryMock.Object, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => provider.UpdateProductDescription(5, "desc"));
        }

        [Fact]
        public async Task Methods_WhenConnectionFactoryThrows_DoNotSwallowException()
        {
            // Arrange
            var ex = new Exception("unable to create connection");
            _dbConnectionFactoryMock.Setup(f => f.CreateConnectionAsync()).ThrowsAsync(ex);

            var provider = new SqlDataProvider(_dbConnectionFactoryMock.Object, _loggerMock.Object);

            // Assert
            await Assert.ThrowsAsync<Exception>(() => provider.GetProductsAsync());
            await Assert.ThrowsAsync<Exception>(() => provider.GetProductByIdAsync(2));
            await Assert.ThrowsAsync<Exception>(() => provider.UpdateProductDescription(2, "x"));
        }
    }
}