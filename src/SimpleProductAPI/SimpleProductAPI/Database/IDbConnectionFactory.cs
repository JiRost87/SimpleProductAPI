using System.Data;

namespace SimpleProductAPI.Database
{
    /// <summary>
    /// Defines operations provided by Connection factory.
    /// </summary>
    public interface IDbConnectionFactory
    {
        /// <summary>
        /// Creates and opens a new <see cref="SqlConnection"/> asynchronously.
        /// </summary>
        /// <returns>An opened <see cref="IDbConnection"/>.</returns>
        Task<IDbConnection> CreateConnectionAsync();
    }
}
