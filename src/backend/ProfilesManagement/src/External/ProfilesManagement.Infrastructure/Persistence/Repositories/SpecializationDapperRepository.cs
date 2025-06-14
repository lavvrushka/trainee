using Dapper;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class SpecializationDapperRepository
        : DapperRepository<Specialization>, ISpecializationRepository
{
    public SpecializationDapperRepository(IDbConnectionFactory factory)
        : base(factory) { }

    public override async Task<List<Specialization>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""Specializations""";
        using var db = Connection;

        return (await db.QueryAsync<Specialization>(sql)).AsList();
    }

    public override async Task<Specialization?> GetByIdAsync(Guid id)
    {
        const string sql = @"
SELECT * 
FROM ""Specializations""
WHERE ""Id"" = @id";
        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Specialization>(sql, new { id });
    }

    public override async Task AddAsync(Specialization entity)
    {
        const string sql = @"
INSERT INTO ""Specializations"" 
  (""Id"", ""Name"", ""Description"")
VALUES
  (@Id, @Name, @Description)";
        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.Name,
            entity.Description
        });
    }

    public override async Task UpdateAsync(Specialization entity)
    {
        const string sql = @"
UPDATE ""Specializations"" SET
  ""Name""        = @Name,
  ""Description"" = @Description
WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            entity.Name,
            entity.Description,
            entity.Id
        });
    }

    public override async Task DeleteAsync(Specialization entity)
    {
        const string sql = @"DELETE FROM ""Specializations"" WHERE ""Id"" = @Id";
        using var db = Connection;

        await db.ExecuteAsync(sql, new { entity.Id });
    }

    public override async Task<int> GetCountAsync()
    {
        const string sql = @"SELECT COUNT(*) FROM ""Specializations""";
        using var db = Connection;

        return await db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<Specialization>> FilterByNameAsync(string name)
    {
        const string sql = @"
SELECT *
FROM ""Specializations""
WHERE ""Name"" ILIKE @p OR ""Description"" ILIKE @p";
        using var db = Connection;

        return (await db.QueryAsync<Specialization>(sql, new { p = $"%{name}%" })).AsList();
    }
}