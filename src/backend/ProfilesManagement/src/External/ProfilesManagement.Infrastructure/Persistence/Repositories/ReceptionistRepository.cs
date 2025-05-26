using Dapper;
using System.Data;
namespace ProfilesManagement.Infrastructure.Persistence.Repositories;

public class ReceptionistDapperRepository : IReceptionistRepository
{
    private readonly IDbConnectionFactory _factory;
    public ReceptionistDapperRepository(IDbConnectionFactory factory)
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

    public async Task<Guid> AddAsync(Receptionist receptionist)
    {
        const string sql = @"
            INSERT INTO ""Receptionists""
              (""Id"", ""FirstName"", ""LastName"", ""MiddleName"", ""Status"", ""AccountId"", ""OfficeId"", ""ImageId"")
            VALUES
              (@Id, @FirstName, @LastName, @MiddleName, @Status, @AccountId, @OfficeId, @ImageId)";
        using var db = Connection;
        await db.ExecuteAsync(sql, receptionist);
        return receptionist.Id;
    }

    public async Task<Receptionist?> GetByIdAsync(Guid id)
    {
        const string sql = @"
            SELECT r.*,
                   i.""Id""        AS Image_Id,
                   i.""ImageData"" AS Image_ImageData,
                   i.""ImageType"" AS Image_ImageType
            FROM ""Receptionists"" r
            LEFT JOIN ""Images"" i ON r.""ImageId"" = i.""Id""
            WHERE r.""Id"" = @id";
        using var db = Connection;
        
        return await db.QueryFirstOrDefaultAsync<Receptionist, Image, Receptionist>(
            sql,
            (r, img) => { r.Image = img; return r; },
            new { id },
            splitOn: "Image_Id"
        );
    }

    public async Task<IEnumerable<Receptionist>> SearchByNameAsync(string name)
    {
        const string sql = @"
            SELECT *
            FROM ""Receptionists""
            WHERE ""FirstName"" ILIKE @p OR ""LastName"" ILIKE @p";
        using var db = Connection;

        return await db.QueryAsync<Receptionist>(sql, new { p = $"%{name}%" });
    }

    public async Task UpdateAsync(Receptionist receptionist)
    {
        const string sql = @"
            UPDATE ""Receptionists""
            SET ""FirstName""   = @FirstName,
                ""LastName""    = @LastName,
                ""MiddleName""  = @MiddleName,
                ""Status""      = @Status,
                ""AccountId""   = @AccountId,
                ""OfficeId""    = @OfficeId,
                ""ImageId""     = @ImageId
            WHERE ""Id"" = @Id";
        using var db = Connection;
        await db.ExecuteAsync(sql, receptionist);
    }

    public Task RemoveAsync(Guid id)
    {
        const string sql = @"DELETE FROM ""Receptionists"" WHERE ""Id"" = @id";
        using var db = Connection;

        return db.ExecuteAsync(sql, new { id });
    }
}
