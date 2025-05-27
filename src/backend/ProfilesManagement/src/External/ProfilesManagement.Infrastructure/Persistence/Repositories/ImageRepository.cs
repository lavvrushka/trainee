using System.Data;
using Dapper;
using ProfilesManagement.Infrastructure.Persistence.Factories;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class ImageDapperRepository : IImageRepository
{
    private readonly IDbConnectionFactory _factory;
    public ImageDapperRepository(IDbConnectionFactory factory)
        => _factory = factory;

    private IDbConnection Connection
    {
        get
        {
            var conn = _factory.CreateConnection();
            conn.Open();

            return conn;
        }
    }

    public async Task<Guid> AddAsync(Image image)
    {
        const string sql = @"
        INSERT INTO ""Images"" (""Id"", ""ImageData"", ""ImageType"")
        VALUES (@Id, @ImageData, @ImageType)";
        using var db = Connection;
        await db.ExecuteAsync(sql, image);

        return image.Id;
    }

    public async Task<Image?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM ""Images"" WHERE ""Id"" = @id";
        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Image>(sql, new { id });
    }

    public Task RemoveAsync(Guid id)
    {
        const string sql = @"DELETE FROM ""Images"" WHERE ""Id"" = @id";
        using var db = Connection;

        return db.ExecuteAsync(sql, new { id });
    }
}
