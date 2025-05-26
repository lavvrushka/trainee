using System.Data;
using Dapper;
using ProfilesManagement.Infrastructure.Persistence.Factories;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories
{
    public class DoctorDapperRepository : IDoctorRepository
    {
        private readonly IDbConnectionFactory _factory;

        public DoctorDapperRepository(IDbConnectionFactory factory)
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

        public async Task<Guid> AddAsync(Doctor doctor)
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

            return doctor.Id;
        }

        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            const string sql = @"
            SELECT d.*, 
                   i.""Id""        AS Image_Id,
                   i.""ImageData"" AS Image_ImageData,
                   i.""ImageType"" AS Image_ImageType
            FROM ""Doctors"" d
            LEFT JOIN ""Images"" i ON d.""ImageId"" = i.""Id""
            WHERE d.""Id"" = @id"
            ;
            using var db = Connection;

            return await db.QueryFirstOrDefaultAsync<Doctor, Image, Doctor>(
                sql,
                map: (d, img) => { d.Image = img; return d; },
                param: new { id },
            splitOn: "Image_Id"
            );
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
            SELECT * FROM ""Doctors""
            WHERE ""SpecializationId"" = @specializationId";
            using var db = Connection;

            return await db.QueryAsync<Doctor>(sql, new { specializationId });
        }

        public async Task<IEnumerable<Doctor>> FilterByOfficeAsync(Guid officeId)
        {
            const string sql = @"
            SELECT * FROM ""Doctors""
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

        public async Task UpdateAsync(Doctor doctor)
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

        public Task RemoveAsync(Guid id)
        {
            const string sql = @"DELETE FROM ""Doctors"" WHERE ""Id"" = @id";
            using var db = Connection;

            return db.ExecuteAsync(sql, new { id });
        }
    }
}
