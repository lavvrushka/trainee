using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace ProfilesManagement.Infrastructure.Persistence.Factories
{
    public class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
