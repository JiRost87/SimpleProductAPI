using Dapper;
using SimpleProductAPI.Database;
using SimpleProductAPI.Models;
using System.Data;

namespace SimpleProductAPI.Data
{
    /// <summary>
    /// SQL-backed implementation of <see cref="IDataProvider"/> that uses Dapper
    /// to call stored procedures for product operations.
    /// </summary>
    public sealed class SqlDataProvider : IDataProvider
    {
        // Stored-procedure names used by this provider.
        private const string _getProductsSpName = "dbo.GetProducts";
        private const string _updateDescriptionSpName = "dbo.InsertOrUpdateProductDescription";

        // Factory used to create IDbConnection instances.
        private readonly IDbConnectionFactory _dbConnectionFactory;

        // Logger for diagnostic information and error reporting.
        private readonly ILogger<SqlDataProvider> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="SqlDataProvider"/>.
        /// </summary>
        /// <param name="dbConnectionFactory">Factory that creates DB connections.</param>
        /// <param name="logger">Logger instance for this class.</param>
        public SqlDataProvider(IDbConnectionFactory dbConnectionFactory, ILogger<SqlDataProvider> logger)
        {
            _dbConnectionFactory = dbConnectionFactory ?? throw new ArgumentNullException(nameof(dbConnectionFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a single <see cref="Product"/> by identifier.
        /// Returns <c>null</c> when no product is found.
        /// </summary>
        /// <param name="id">Product identifier.</param>
        /// <returns>The product or <c>null</c> if not found.</returns>
        /// <exception cref="Exception">Any exception from the data access layer is rethrown after logging.</exception>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogDebug("GetProductByIdAsync: id={Id} - opening connection", id);

            try
            {
                // Create and open a connection via the factory. 'using var' ensures disposal.
                using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);

                // Call stored procedure to fetch a single product by id.
                var product = await dbConnection.QuerySingleOrDefaultAsync<Product>(
                    _getProductsSpName,
                    new { ProductId = id },
                    commandType: CommandType.StoredProcedure
                ).ConfigureAwait(false);

                // Log whether a product was found.
                _logger.LogDebug("GetProductByIdAsync: id={Id} - returned {Found}", id, product != null);

                return product;
            }
            catch (Exception ex)
            {
                // Log the exception with contextual information then rethrow.
                _logger.LogError(ex, "GetProductByIdAsync: error for id={Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all products from the database.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Product}"/> containing all products.</returns>
        /// <exception cref="Exception">Any exception from the data access layer is rethrown after logging.</exception>
        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            _logger.LogDebug("GetProductsAsync: opening connection to retrieve products");

            try
            {
                // Obtain a connection and execute the stored procedure that returns product rows.
                using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);
                var products = await dbConnection.QueryAsync<Product>(
                    _getProductsSpName,
                    commandType: CommandType.StoredProcedure
                ).ConfigureAwait(false);

                // Log the number of items retrieved. Use AsList() to get a count efficiently.
                _logger.LogDebug("GetProductsAsync: retrieved {Count} items", products?.AsList().Count ?? 0);

                return products ?? Enumerable.Empty<Product>();
            }
            catch (Exception ex)
            {
                // Log and rethrow to allow higher layers to handle the exception.
                _logger.LogError(ex, "GetProductsAsync: error retrieving products");
                throw;
            }
        }

        /// <summary>
        /// Inserts or updates the product description for the specified product id.
        /// </summary>
        /// <param name="id">Product identifier to update.</param>
        /// <param name="description">New product description.</param>
        /// <returns><c>true</c> when the update affected exactly one row; otherwise <c>false</c>.</returns>
        /// <exception cref="Exception">Any exception from the data access layer is rethrown after logging.</exception>
        public async Task<bool> UpdateProductDescription(int id, string description)
        {
            _logger.LogDebug("UpdateProductDescription: id={Id} - attempting update", id);

            // Parameter object passed to the stored procedure.
            var parameters = new { ProductId = id, Description = description };

            try
            {
                using var dbConnection = await _dbConnectionFactory.CreateConnectionAsync().ConfigureAwait(false);

                // Execute the stored procedure that performs an insert or update.
                var result = await dbConnection.ExecuteAsync(
                    _updateDescriptionSpName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ConfigureAwait(false);

                // Succeeded if exactly one row was affected.
                var succeeded = result == 1;

                _logger.LogDebug("UpdateProductDescription: id={Id} - result={Result}", id, succeeded);

                return succeeded;
            }
            catch (Exception ex)
            {
                // Log and rethrow to preserve stack trace for the caller.
                _logger.LogError(ex, "UpdateProductDescription: error updating id={Id}", id);
                throw;
            }
        }
    }
}