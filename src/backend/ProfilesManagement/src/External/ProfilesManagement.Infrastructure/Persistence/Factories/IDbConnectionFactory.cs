using System.Data;


namespace ProfilesManagement.Infrastructure.Persistence.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
