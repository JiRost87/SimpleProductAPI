using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using System.Data;

namespace SimpleProductAPI.Database
{
    /// <summary>
    /// Factory that creates and opens <see cref="IDbConnection"/> instances for SQL Server.
    /// Uses <see cref="SqlConnection"/> and opens the connection asynchronously.
    /// Implements a Polly retry policy for transient connection failures.
    /// </summary>
    public class SqlServerConnectionFactory : IDbConnectionFactory
    {
        // Connection string used to create the SqlConnection.
        private readonly string _connectionString;

        // Optional logger for diagnostics; null when logging is not required/configured.
        private readonly ILogger<SqlServerConnectionFactory>? _logger;

        // Retry policy used when opening connections.
        private readonly AsyncRetryPolicy _retryPolicy;

        /// <summary>
        /// Initializes a new instance of <see cref="SqlServerConnectionFactory"/>.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <param name="logger">Logger</param>
        public SqlServerConnectionFactory(string connectionString, ILogger<SqlServerConnectionFactory> logger)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configure a Polly retry policy for transient exceptions when opening connections.
            // - Retries 3 times with exponential backoff (2^attempt seconds).
            // - Handles SqlException, TimeoutException and InvalidOperationException (common open-time errors).
            _retryPolicy = Policy
                .Handle<SqlException>()
                .Or<TimeoutException>()
                .Or<InvalidOperationException>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // 2s, 4s, 8s
                    onRetry: (exception, timespan, attempt, context) =>
                    {
                        _logger?.LogWarning(exception, "Attempt {Attempt} to open SQL connection failed. Waiting {Delay} before retry.", attempt, timespan);
                    });
        }

        /// <summary>
        /// Creates and opens a new <see cref="SqlConnection"/> asynchronously with retry semantics.
        /// </summary>
        /// <returns>An opened <see cref="IDbConnection"/>.</returns>
        /// <exception cref="Exception">Any exception thrown while opening the connection is logged then rethrown.</exception>
        public async Task<IDbConnection> CreateConnectionAsync()
        {
            _logger?.LogDebug("Creating SQL connection. Connecting to datasource.");

            // Create the SqlConnection instance.
            var connection = new SqlConnection(_connectionString);

            try
            {
                // Use Polly retry policy when opening the connection to handle transient errors.
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);

                _logger?.LogDebug("SQL connection opened successfully.");
                return connection;
            }
            catch (Exception ex)
            {
                // Log the error with the exception details, dispose the connection to free resources,
                // then rethrow to let callers handle the failure.
                _logger?.LogError(ex, "Failed to open SQL connection after retries.");
                connection.Dispose();
                throw;
            }
        }
    }
}
