using Dapper;
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
            SELECT p.*,
                   i.""Id""        AS Image_Id,
                   i.""ImageData"" AS Image_ImageData,
                   i.""ImageType"" AS Image_ImageType
            FROM ""Patients"" p
            LEFT JOIN ""Images"" i ON p.""ImageId"" = i.""Id""
            WHERE p.""Id"" = @id";
        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Patient, Image, Patient>(
            sql,
            map: (p, img) =>
            {
                p.Image = img;
                return p;
            },
            new { id },
            splitOn: "Image_Id"
        );
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

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        const string sql = @"
            SELECT *
            FROM ""Patients""
            WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";
        using var db = Connection;

        return await db.QueryAsync<Patient>(sql, new { p = $"%{name}%" });
    }
}
