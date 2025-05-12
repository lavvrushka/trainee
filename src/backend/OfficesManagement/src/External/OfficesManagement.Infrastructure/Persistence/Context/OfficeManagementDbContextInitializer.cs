using OfficesManagement.Infrastructure.Persistence.Contexts;
namespace OfficesManagement.DataAccess.Persistence.Context;

public static class OfficeManagementDbContextInitializer
{
    public static void Initialize()
    {
        MongoMappings.Register();
    }
}