using Dapper;
using SimpleProductAPI.Database;
using SimpleProductAPI.Models;
using System.Data;

namespace SimpleProductAPI.Data
{
    public sealed class SqlDataProvider : IDataProvider
    {

        private const string _getProductsSpName = "dbo.GetProducts";
        private const string _updateDescriptionSpName = "dbo.InsertOrUpdateProductDescription";

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlDataProvider(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);
            var product = await dbConnection.QuerySingleOrDefaultAsync<Product>(_getProductsSpName, new { ProductId = id }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);
            var products = await dbConnection.QueryAsync<Product>(_getProductsSpName, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return products;
        }

        public async Task<bool> UpdateProductDescription(int id, string description)
        {
            var parameters = new { ProductId = id, Description = description };

            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);
            var result = await dbConnection.ExecuteAsync(_updateDescriptionSpName, parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result == 1;
        }
    }
}