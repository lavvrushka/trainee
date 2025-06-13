using Dapper;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class ImageDapperRepository
    : DapperRepository<Image>, IImageRepository
{
    public ImageDapperRepository(IDbConnectionFactory factory)
        : base(factory)
    {
    }

    public override async Task<List<Image>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""Images""";
        using var db = Connection;
        var result = await db.QueryAsync<Image>(sql);

        return result.AsList();
    }

    public override async Task<Image?> GetByIdAsync(Guid id)
    {
        const string sql = @"SELECT * FROM ""Images"" WHERE ""Id"" = @id";
        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Image>(sql, new { id });
    }

    public override async Task AddAsync(Image image)
    {
        const string sql = @"
                INSERT INTO ""Images"" (""Id"", ""ImageData"", ""ImageType"")
                VALUES (@Id, @ImageData, @ImageType)";
        using var db = Connection;
        await db.ExecuteAsync(sql, image);
    }
    public override async Task UpdateAsync(Image image)
    {
        const string sql = @"
                UPDATE ""Images""
                SET ""ImageData"" = @ImageData,
                    ""ImageType"" = @ImageType
                WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, image);
    }
    public override Task DeleteAsync(Image image)
    {
        const string sql = @"DELETE FROM ""Images"" WHERE ""Id"" = @Id";
        using var db = Connection;

        return db.ExecuteAsync(sql, new { image.Id });
    }
}
