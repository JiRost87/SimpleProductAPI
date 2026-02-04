using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleProductAPI.Database;
using Xunit;

namespace SimpleProductAPI.Tests.Data
{
    public class SqlServerConnectionFactoryTests
    {
        [Fact]
        public void Ctor_NullConnectionString_ThrowsArgumentNullException()
        {
            var logger = new Mock<ILogger<SqlServerConnectionFactory>>();
            Assert.Throws<ArgumentNullException>(() => new SqlServerConnectionFactory(null!, logger.Object));
        }

        [Fact]
        public void Ctor_NullLogger_ThrowsArgumentNullException()
        {
            var connectionString = "Server=.;Database=master;Integrated Security=true;";
            Assert.Throws<ArgumentNullException>(() => new SqlServerConnectionFactory(connectionString, null!));
        }
    }
}