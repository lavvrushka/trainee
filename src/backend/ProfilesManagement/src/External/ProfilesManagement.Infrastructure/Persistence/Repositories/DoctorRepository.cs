using Dapper;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
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
        return (await db.QueryAsync<Doctor>(sql)).AsList();
    }
    public override async Task<Doctor?> GetByIdAsync(Guid id)
    {
        const string sql = @"
SELECT d.*,
    s.""Id""           AS Specialization_Id,
    s.""Name""         AS Specialization_Name,
    s.""Description""  AS Specialization_Description,
    st.""Id""          AS Status_Id,
    st.""Status""      AS Status_Status,
    st.""Description"" AS Status_Description,
    st.""CreatedAt""   AS Status_CreatedAt
FROM ""Doctors"" d
LEFT JOIN ""Specializations"" s ON d.""SpecializationId"" = s.""Id""
LEFT JOIN ""EmploymentStatuses"" st ON d.""StatusId"" = st.""Id""
WHERE d.""Id"" = @id";

        using var db = Connection;
        var list = await db.QueryAsync<Doctor, Specialization, EmploymentStatus, Doctor>(
            sql,
            (doc, spec, st) =>
            {
                doc.Specialization = spec;
                doc.Status = st;
                return doc;
            },
            new { id },
            splitOn: "Specialization_Id,Status_Id"
        );

        return list.FirstOrDefault();
    }

    public override async Task AddAsync(Doctor doctor)
    {
        const string sql = @"
INSERT INTO ""Doctors"" 
(""Id"", ""FirstName"", ""LastName"", ""MiddleName"",
""AccountId"", ""OfficeId"", ""SpecializationId"",
""CareerStartYear"", ""StatusId"", ""ImageId"")
VALUES
(@Id, @FirstName, @LastName, @MiddleName,
@AccountId, @OfficeId, @SpecializationId,
@CareerStartYear, @StatusId, @ImageId)";

        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            doctor.Id,
            doctor.FirstName,
            doctor.LastName,
            doctor.MiddleName,
            doctor.AccountId,
            doctor.OfficeId,
            doctor.SpecializationId,
            doctor.CareerStartYear,
            doctor.StatusId,
            doctor.ImageId
        });
    }
    public override async Task UpdateAsync(Doctor doctor)
    {
        const string sql = @"
UPDATE ""Doctors"" SET
""FirstName""        = @FirstName,
""LastName""         = @LastName,
""MiddleName""       = @MiddleName,
""AccountId""        = @AccountId,
""OfficeId""         = @OfficeId,
""SpecializationId"" = @SpecializationId,
""CareerStartYear""  = @CareerStartYear,
""StatusId""         = @StatusId,
""ImageId""          = @ImageId
WHERE ""Id"" = @Id";

        using var db = Connection;
        await db.ExecuteAsync(sql, new
        {
            doctor.FirstName,
            doctor.LastName,
            doctor.MiddleName,
            doctor.AccountId,
            doctor.OfficeId,
            doctor.SpecializationId,
            doctor.CareerStartYear,
            doctor.StatusId,
            doctor.ImageId,
            doctor.Id
        });
    }
    public override Task DeleteAsync(Doctor doctor)
    {
        const string sql = @"DELETE FROM ""Doctors"" WHERE ""Id"" = @Id";
        using var db = Connection;
        return db.ExecuteAsync(sql, new { doctor.Id });
    }

    public override async Task<int> GetCountAsync()
    {
        const string sql = @"SELECT COUNT(*) FROM ""Doctors""";
        using var db = Connection;
        return await db.ExecuteScalarAsync<int>(sql);
    }

    public async Task<List<Doctor>> GetByPageAsync(PageSettings pageSettings)
    {
        const string sql = @"
SELECT * 
FROM ""Doctors""
ORDER BY ""LastName"", ""FirstName""
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        using var db = Connection;
        var items = await db.QueryAsync<Doctor>(sql, new
        {
            Offset = (pageSettings.PageIndex - 1) * pageSettings.PageSize,
            PageSize = pageSettings.PageSize
        });
        return items.AsList();
    }

    public async Task<List<Doctor>> FilterByNameAsync(string name)
    {
        const string sql = @"
SELECT *
FROM ""Doctors""
WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";

        using var db = Connection;
        return (await db.QueryAsync<Doctor>(sql, new { p = $"%{name}%" })).AsList();
    }

    public async Task<List<Doctor>> FilterBySpecializationAsync(Guid specializationId)
    {
        const string sql = @"
SELECT *
FROM ""Doctors""
WHERE ""SpecializationId"" = @specializationId";

        using var db = Connection;
        return (await db.QueryAsync<Doctor>(sql, new { specializationId })).AsList();
    }

    public async Task<List<Doctor>> FilterByOfficeAsync(Guid officeId)
    {
        const string sql = @"
SELECT *
FROM ""Doctors""
WHERE ""OfficeId"" = @officeId";

        using var db = Connection;
        return (await db.QueryAsync<Doctor>(sql, new { officeId })).AsList();
    }

    public Task ChangeStatusAsync(Guid id, string status)
    {
        const string sql = @"
UPDATE ""Doctors""
SET ""StatusId"" = @statusId
WHERE ""Id"" = @id";

        using var db = Connection;
        return db.ExecuteAsync(sql, new { statusId = status, id });
    }
}


