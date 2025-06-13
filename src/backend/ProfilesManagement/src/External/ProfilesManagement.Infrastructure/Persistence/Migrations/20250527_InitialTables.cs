using FluentMigrator;
namespace ProfilesManagement.Infrastructure.Persistence.Migrations;

[Migration(2025052701)]
public class InitialTables : Migration
{
    public override void Up()
    {
        Create.Table("Specializations")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(200).NotNullable()
            .WithColumn("Description").AsString(1000).Nullable();

        Create.Table("EmploymentStatuses")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Status").AsString(100).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable();

        Create.Table("Doctors")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsString(100).NotNullable()
            .WithColumn("LastName").AsString(100).NotNullable()
            .WithColumn("MiddleName").AsString(100).NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("OfficeId").AsGuid().Nullable()
            .WithColumn("SpecializationId").AsGuid().Nullable()
            .WithColumn("CareerStartYear").AsDateTime().NotNullable()
            .WithColumn("StatusId").AsGuid().NotNullable()
            .WithColumn("ImageId").AsGuid().Nullable();

        Create.ForeignKey("FK_Doctors_Specialization")
            .FromTable("Doctors").ForeignColumn("SpecializationId")
            .ToTable("Specializations").PrimaryColumn("Id");

        Create.ForeignKey("FK_Doctors_Status")
            .FromTable("Doctors").ForeignColumn("StatusId")
            .ToTable("EmploymentStatuses").PrimaryColumn("Id");


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
            .WithColumn("StatusId").AsGuid().NotNullable()
            .WithColumn("AccountId").AsGuid().NotNullable()
            .WithColumn("OfficeId").AsGuid().Nullable()
            .WithColumn("ImageId").AsGuid().Nullable();

        Create.ForeignKey("FK_Receptionists_Status")
            .FromTable("Receptionists").ForeignColumn("StatusId")
            .ToTable("EmploymentStatuses").PrimaryColumn("Id");
    }

    public override void Down()
    {
        Delete.Table("Receptionists");
        Delete.Table("Patients");
        Delete.Table("Doctors");
        Delete.Table("EmploymentStatuses");
        Delete.Table("Specializations");
    }
}
