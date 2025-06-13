using FluentMigrator;
namespace ProfilesManagement.Infrastructure.Migrations;

[Migration(2025052702)]
public class CreateIndexes : Migration
{
    public override void Up()
    {
        Create.Index("IX_Doctors_Name")
            .OnTable("Doctors")
            .OnColumn("LastName").Ascending()
            .OnColumn("FirstName").Ascending();

        Create.Index("IX_Patients_Name")
            .OnTable("Patients")
            .OnColumn("LastName").Ascending()
            .OnColumn("FirstName").Ascending();

        Create.Index("IX_Doctors_MiddleName")
            .OnTable("Doctors")
            .OnColumn("MiddleName").Ascending();

        Create.Index("IX_Patients_MiddleName")
            .OnTable("Patients")
            .OnColumn("MiddleName").Ascending();

        Create.Index("IX_Doctors_SpecializationId")
            .OnTable("Doctors")
            .OnColumn("SpecializationId").Ascending();

        Create.Index("IX_Doctors_StatusId")
            .OnTable("Doctors")
            .OnColumn("StatusId").Ascending();

        Create.Index("IX_Doctors_OfficeId")
            .OnTable("Doctors")
            .OnColumn("OfficeId").Ascending();

        Create.Index("IX_Doctors_AccountId")
            .OnTable("Doctors")
            .OnColumn("AccountId").Ascending();

        Create.Index("IX_Receptionists_StatusId")
            .OnTable("Receptionists")
            .OnColumn("StatusId").Ascending();

        Create.Index("IX_Receptionists_OfficeId")
            .OnTable("Receptionists")
            .OnColumn("OfficeId").Ascending();

        Create.Index("IX_Receptionists_AccountId")
            .OnTable("Receptionists")
            .OnColumn("AccountId").Ascending();

        Create.Index("IX_Patients_AccountId")
            .OnTable("Patients")
            .OnColumn("AccountId").Ascending();
    }

    public override void Down()
    {
        Delete.Index("IX_Patients_AccountId").OnTable("Patients");
        Delete.Index("IX_Patients_MiddleName").OnTable("Patients");
        Delete.Index("IX_Patients_Name").OnTable("Patients");

        Delete.Index("IX_Doctors_AccountId").OnTable("Doctors");
        Delete.Index("IX_Doctors_OfficeId").OnTable("Doctors");
        Delete.Index("IX_Doctors_StatusId").OnTable("Doctors");
        Delete.Index("IX_Doctors_SpecializationId").OnTable("Doctors");
        Delete.Index("IX_Doctors_MiddleName").OnTable("Doctors");
        Delete.Index("IX_Doctors_Name").OnTable("Doctors");

        Delete.Index("IX_Receptionists_AccountId").OnTable("Receptionists");
        Delete.Index("IX_Receptionists_OfficeId").OnTable("Receptionists");
        Delete.Index("IX_Receptionists_StatusId").OnTable("Receptionists");
    }
}
