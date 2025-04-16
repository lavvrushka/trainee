using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UserManagement.Infrastructure.Persistence.Context
{
    public class UserManagementDbContextFactory : IDesignTimeDbContextFactory<UserManagementDbContext>
    {
        public UserManagementDbContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("C:\\Users\\bebrik\\Desktop\\trainee\\src\\backend\\UserManagement\\src\\External\\UserManagement.API\\appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<UserManagementDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new UserManagementDbContext(optionsBuilder.Options);
        }
    }
}