using Dapper;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class ReceptionistDapperRepository
    : DapperRepository<Receptionist>, IReceptionistRepository
{
    public ReceptionistDapperRepository(IDbConnectionFactory factory)
        : base(factory)
    {
    }

    public override async Task<List<Receptionist>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""Receptionists""";
        using var db = Connection;
        var list = await db.QueryAsync<Receptionist>(sql);
        return list.AsList();
    }

    public override async Task<Receptionist?> GetByIdAsync(Guid id)
    {
        const string sql = @"
SELECT *
FROM ""Receptionists""
WHERE ""Id"" = @id";

        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<Receptionist>(sql, new { id });
    }

    public override async Task AddAsync(Receptionist receptionist)
    {
        const string sql = @"
INSERT INTO ""Receptionists""
  (""Id"", ""FirstName"", ""LastName"", ""MiddleName"",
   ""StatusId"", ""AccountId"", ""OfficeId"", ""ImageId"")
VALUES
  (@Id, @FirstName, @LastName, @MiddleName,
   @StatusId, @AccountId, @OfficeId, @ImageId)";
        using var db = Connection;
        await db.ExecuteAsync(sql, receptionist);
    }

    public override async Task UpdateAsync(Receptionist receptionist)
    {
        const string sql = @"
UPDATE ""Receptionists""
SET ""FirstName""   = @FirstName,
    ""LastName""    = @LastName,
    ""MiddleName""  = @MiddleName,
    ""StatusId""    = @StatusId,
    ""AccountId""   = @AccountId,
    ""OfficeId""    = @OfficeId,
    ""ImageId""     = @ImageId
WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, receptionist);
    }

    public override Task DeleteAsync(Receptionist receptionist)
    {
        const string sql = @"DELETE FROM ""Receptionists"" WHERE ""Id"" = @Id";
        using var db = Connection;
        return db.ExecuteAsync(sql, new { receptionist.Id });
    }

    public override async Task<int> GetCountAsync()
    {
        const string sql = @"SELECT COUNT(*) FROM ""Receptionists""";
        using var db = Connection;
        return await db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<Receptionist>> SearchByNameAsync(string name)
    {
        const string sql = @"
SELECT *
FROM ""Receptionists""
WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";
        using var db = Connection;
        var list = await db.QueryAsync<Receptionist>(sql, new { p = $"%{name}%" });
        return list.AsList();
    }

    public async Task<List<Receptionist>> GetByPageAsync(PageSettings pageSettings)
    {
        const string sql = @"
SELECT *
FROM ""Receptionists""
ORDER BY ""LastName"", ""FirstName""
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        using var db = Connection;
        var list = await db.QueryAsync<Receptionist>(sql, new
        {
            Offset = (pageSettings.PageIndex - 1) * pageSettings.PageSize,
            PageSize = pageSettings.PageSize
        });
        return list.AsList();
    }
}