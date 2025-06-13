using Dapper;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class PatientDapperRepository
    : DapperRepository<Patient>, IPatientRepository
{
    public PatientDapperRepository(IDbConnectionFactory factory)
        : base(factory)
    {
    }

    public override async Task<List<Patient>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""Patients""";
        using var db = Connection;
        var result = await db.QueryAsync<Patient>(sql);
        return result.AsList();
    }

    public override async Task<Patient?> GetByIdAsync(Guid id)
    {
        const string sql = @"
SELECT *
FROM ""Patients""
WHERE ""Id"" = @id";

        using var db = Connection;
        return await db.QueryFirstOrDefaultAsync<Patient>(sql, new { id });
    }

    public override async Task AddAsync(Patient patient)
    {
        const string sql = @"
INSERT INTO ""Patients""
  (""Id"", ""FirstName"", ""LastName"", ""MiddleName"", ""AccountId"", ""ImageId"")
VALUES
  (@Id, @FirstName, @LastName, @MiddleName, @AccountId, @ImageId)";
        using var db = Connection;
        await db.ExecuteAsync(sql, patient);
    }

    public override async Task UpdateAsync(Patient patient)
    {
        const string sql = @"
UPDATE ""Patients""
SET ""FirstName""  = @FirstName,
    ""LastName""   = @LastName,
    ""MiddleName"" = @MiddleName,
    ""AccountId""  = @AccountId,
    ""ImageId""    = @ImageId
WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, patient);
    }

    public override Task DeleteAsync(Patient patient)
    {
        const string sql = @"DELETE FROM ""Patients"" WHERE ""Id"" = @Id";
        using var db = Connection;
        return db.ExecuteAsync(sql, new { patient.Id });
    }

    public override async Task<int> GetCountAsync()
    {
        const string sql = @"SELECT COUNT(*) FROM ""Patients""";
        using var db = Connection;
        return await db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<Patient>> SearchByNameAsync(string name)
    {
        const string sql = @"
SELECT *
FROM ""Patients""
WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";
        using var db = Connection;
        var result = await db.QueryAsync<Patient>(sql, new { p = $"%{name}%" });
        return result.AsList();
    }

    public async Task<List<Patient>> GetByPageAsync(PageSettings pageSettings)
    {
        const string sql = @"
SELECT *
FROM ""Patients""
ORDER BY ""LastName"", ""FirstName""
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        using var db = Connection;
        var result = await db.QueryAsync<Patient>(sql, new
        {
            Offset = (pageSettings.PageIndex - 1) * pageSettings.PageSize,
            PageSize = pageSettings.PageSize
        });
        return result.AsList();
    }
}
