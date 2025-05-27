using System.Data;
using Dapper;
using ProfilesManagement.Infrastructure.Persistence.Factories;

namespace ProfilesManagement.Infrastructure.Persistence.Repositories
{
    public class PatientDapperRepository : IPatientRepository
    {
        private readonly IDbConnectionFactory _factory;
        public PatientDapperRepository(IDbConnectionFactory factory)
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

        public async Task<Guid> AddAsync(Patient patient)
        {
            const string sql = @"
            INSERT INTO ""Patients""
              (""Id"", ""FirstName"", ""LastName"", ""MiddleName"", ""AccountId"", ""ImageId"")
            VALUES
              (@Id, @FirstName, @LastName, @MiddleName, @AccountId, @ImageId)";
            using var db = Connection;
            await db.ExecuteAsync(sql, patient);

            return patient.Id;
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
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
                (p, img) => { p.Image = img; return p; },
                new { id },
                splitOn: "Image_Id"
            );
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

        public async Task UpdateAsync(Patient patient)
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

        public Task RemoveAsync(Guid id)
        {
            const string sql = @"DELETE FROM ""Patients"" WHERE ""Id"" = @id";
            using var db = Connection;

            return db.ExecuteAsync(sql, new { id });
        }
    }
}
