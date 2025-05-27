using FluentMigrator;
namespace ProfilesManagement.Infrastructure.Persistence.Migrations;

[Migration(2025052701)]
public class InitialTables : Migration
{
    public override void Up()
    {
        Create.Table("Images")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("ImageData").AsString(int.MaxValue).NotNullable()
            .WithColumn("ImageType").AsString(50).NotNullable();

        Create.Table("Doctors")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsString(100).NotNullable()
            .WithColumn("LastName").AsString(100).NotNullable()
            .WithColumn("MiddleName").AsString(100).NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("OfficeId").AsGuid().Nullable()
            .WithColumn("SpecializationId").AsGuid().Nullable()
            .WithColumn("CareerStartYear").AsDateTime().NotNullable()
            .WithColumn("Status").AsString(50).NotNullable()
            .WithColumn("ImageId").AsGuid().Nullable();

        Create.Table("Patients")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsString(100).NotNullable()
            .WithColumn("LastName").AsString(100).NotNullable()
            .WithColumn("MiddleName").AsString(100).NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("ImageId").AsGuid().Nullable();

        Create.Table("Receptionists")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsString(100).NotNullable()
            .WithColumn("LastName").AsString(100).NotNullable()
            .WithColumn("MiddleName").AsString(100).NotNullable()
            .WithColumn("Status").AsString(50).NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("OfficeId").AsGuid().Nullable()
            .WithColumn("ImageId").AsGuid().Nullable();
    }

    public override void Down()
    {
        Delete.Table("Receptionists");
        Delete.Table("Patients");
        Delete.Table("Doctors");
        Delete.Table("Images");
    }
}
