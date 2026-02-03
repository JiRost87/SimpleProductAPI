using Microsoft.Data.SqlClient;
using System.Data;

namespace SimpleProductAPI.Database
{
    /// <summary>
    /// Factory that creates and opens <see cref="IDbConnection"/> instances for SQL Server.
    /// Uses <see cref="SqlConnection"/> and opens the connection asynchronously.
    /// </summary>
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        // Connection string used to create the SqlConnection.
        private readonly string _connectionString;

        // Optional logger for diagnostics; null when logging is not required/configured.
        private readonly ILogger<SqlServerConnectionFactory>? _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="SqlServerConnectionFactory"/>.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <param name="logger">Optional logger; can be null.</param>
        public SqlServerConnectionFactory(string connectionString, ILogger<SqlServerConnectionFactory>? logger = null)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        /// <summary>
        /// Creates and opens a new <see cref="SqlConnection"/> asynchronously.
        /// </summary>
        /// <returns>An opened <see cref="IDbConnection"/>.</returns>
        /// <exception cref="Exception">Any exception thrown while opening the connection is logged then rethrown.</exception>
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            // Log intent to create and open a connection if a logger is available.
            _logger?.LogDebug("Creating SQL connection. Connecting to datasource.");

            // Create the SqlConnection instance; the Dispose/Close lifetime is managed by the caller.
            var connection = new SqlConnection(_connectionString);

            try
            {
                // Open the connection asynchronously.
                await connection.OpenAsync().ConfigureAwait(false);

                // Log success and return the opened connection.
                _logger?.LogDebug("SQL connection opened successfully.");
                return connection;
            }
            catch (Exception ex)
            {
                // Log the error with the exception details, dispose the connection to free resources,
                // then rethrow to let callers handle the failure.
                _logger?.LogError(ex, "Failed to open SQL connection.");
                connection.Dispose();
                throw;
            }
        }
    }
}
