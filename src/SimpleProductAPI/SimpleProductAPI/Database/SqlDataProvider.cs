using Dapper;
using SimpleProductAPI.Database;
using SimpleProductAPI.Models;
using System.Data;

namespace SimpleProductAPI.Data
{
    public class SqlDataProvider : IDataProvider
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
            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync();
            var product = await dbConnection.QueryFirstOrDefaultAsync<Product>(_getProductsSpName, id, commandType: CommandType.StoredProcedure);
            return product;
        } 

        public async Task<IEnumerable<Product>> GetProducts()
        {
            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync();
            var products = await dbConnection.QueryAsync<Product>(_getProductsSpName,commandType: CommandType.StoredProcedure);
            return products;
        }

        public async Task<bool> UpdateProductDescription(int id, string description) 
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("id", id);
            dp.Add("description", description);

            using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync();
            var result = await dbConnection.ExecuteAsync(_updateDescriptionSpName, dp, commandType: CommandType.StoredProcedure);
            if (result != 1)
            {
                return false;
            }
            return true;
        }


    }
}
