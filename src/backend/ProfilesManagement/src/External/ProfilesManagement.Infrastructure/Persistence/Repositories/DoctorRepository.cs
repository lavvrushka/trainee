using Dapper;
using ProfilesManagement.Domain.Models;
using ProfilesManagement.Infrastructure.Persistence.Factories;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class DoctorDapperRepository
    : DapperRepository<Doctor>, IDoctorRepository
{
    public DoctorDapperRepository(IDbConnectionFactory factory)
        : base(factory)
    {
    }
    public override async Task<List<Doctor>> GetAllAsync()
    {
        const string sql = @"SELECT * FROM ""Doctors""";
        using var db = Connection;
        var result = await db.QueryAsync<Doctor>(sql);
        return result.AsList();
    }
    public override async Task<Doctor?> GetByIdAsync(Guid id)
    {
        const string sql = @"
                SELECT d.*, 
                       i.""Id""        AS Image_Id,
                       i.""ImageData"" AS Image_ImageData,
                       i.""ImageType"" AS Image_ImageType
                FROM ""Doctors"" d
                LEFT JOIN ""Images"" i ON d.""ImageId"" = i.""Id""
                WHERE d.""Id"" = @id";
        using var db = Connection;

        return await db.QueryFirstOrDefaultAsync<Doctor, Image, Doctor>(
            sql,
            map: (d, img) =>
            {
                d.Image = img;
                return d;
            },
            param: new { id },
            splitOn: "Image_Id"
        );
    }

    public override async Task AddAsync(Doctor doctor)
    {
        const string sql = @"
                INSERT INTO ""Doctors"" 
                  (""Id"", ""FirstName"", ""LastName"", ""MiddleName"", ""AccountId"", 
                   ""OfficeId"", ""SpecializationId"", ""CareerStartYear"", ""Status"", ""ImageId"")
                VALUES
                  (@Id, @FirstName, @LastName, @MiddleName, @AccountId, 
                   @OfficeId, @SpecializationId, @CareerStartYear, @Status, @ImageId)";
        using var db = Connection;
        await db.ExecuteAsync(sql, doctor);
    }

    public override async Task UpdateAsync(Doctor doctor)
    {
        const string sql = @"
                UPDATE ""Doctors""
                SET ""FirstName""        = @FirstName,
                    ""LastName""         = @LastName,
                    ""MiddleName""       = @MiddleName,
                    ""AccountId""        = @AccountId,
                    ""OfficeId""         = @OfficeId,
                    ""SpecializationId"" = @SpecializationId,
                    ""CareerStartYear""  = @CareerStartYear,
                    ""Status""           = @Status,
                    ""ImageId""          = @ImageId
                WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, doctor);
    }

    public override Task DeleteAsync(Doctor doctor)
    {
        const string sql = @"DELETE FROM ""Doctors"" WHERE ""Id"" = @Id";
        using var db = Connection;
        
        return db.ExecuteAsync(sql, new { doctor.Id });
    }

    public async Task<IEnumerable<Doctor>> SearchByNameAsync(string name)
    {
        const string sql = @"
                SELECT *
                FROM ""Doctors""
                WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";
        using var db = Connection;

        return await db.QueryAsync<Doctor>(sql, new { p = $"%{name}%" });
    }

    public async Task<IEnumerable<Doctor>> FilterBySpecializationAsync(Guid specializationId)
    {
        const string sql = @"
                SELECT *
                FROM ""Doctors""
                WHERE ""SpecializationId"" = @specializationId";
        using var db = Connection;

        return await db.QueryAsync<Doctor>(sql, new { specializationId });
    }

    public async Task<IEnumerable<Doctor>> FilterByOfficeAsync(Guid officeId)
    {
        const string sql = @"
                SELECT *
                FROM ""Doctors""
                WHERE ""OfficeId"" = @officeId";
        using var db = Connection;

        return await db.QueryAsync<Doctor>(sql, new { officeId });
    }

    public Task ChangeStatusAsync(Guid id, string status)
    {
        const string sql = @"
                UPDATE ""Doctors""
                SET ""Status"" = @status
                WHERE ""Id"" = @id";
        using var db = Connection;

        return db.ExecuteAsync(sql, new { id, status });
    }
}
