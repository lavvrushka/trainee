using Dapper;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class EmploymentStatusDapperRepository
    : DapperRepository<EmploymentStatus>, IEmploymentStatusRepository
{
    public EmploymentStatusDapperRepository(IDbConnectionFactory factory)
        : base(factory)
    {
    }

    public override async Task<List<EmploymentStatus>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""EmploymentStatuses""";
        using var db = Connection;
        var list = await db.QueryAsync<EmploymentStatus>(sql);
        return list.AsList();
    }

    public override async Task<EmploymentStatus?> GetByIdAsync(Guid id)
    {
        const string sql = @"
SELECT *
FROM ""EmploymentStatuses""
WHERE ""Id"" = @id";
        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<EmploymentStatus>(sql, new { id });
    }

    public override async Task AddAsync(EmploymentStatus entity)
    {
        const string sql = @"
INSERT INTO ""EmploymentStatuses""
  (""Id"", ""Status"", ""Description"", ""CreatedAt"")
VALUES
  (@Id, @Status, @Description, @CreatedAt)";
        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            entity.Id,
            entity.Status,
            entity.Description,
            entity.DateTime
        });
    }

    public override async Task UpdateAsync(EmploymentStatus entity)
    {
        const string sql = @"
UPDATE ""EmploymentStatuses"" SET
  ""Status""      = @Status,
  ""Description"" = @Description
WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            entity.Status,
            entity.Description,
            entity.Id
        });
    }

    public override async Task DeleteAsync(EmploymentStatus entity)
    {
        const string sql = @"DELETE FROM ""EmploymentStatuses"" WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, new { entity.Id });
    }

    public override async Task<int> GetCountAsync()
    {
        const string sql = @"SELECT COUNT(*) FROM ""EmploymentStatuses""";
        using var db = Connection;
        return await db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<EmploymentStatus>> FilterByStatusNameAsync(string name)
    {
        const string sql = @"
SELECT *
FROM ""EmploymentStatuses""
WHERE ""Status"" ILIKE @p OR ""Description"" ILIKE @p";
        using var db = Connection;
        var list = await db.QueryAsync<EmploymentStatus>(sql, new { p = $"%{name}%" });
        return list.AsList();
    }
}
