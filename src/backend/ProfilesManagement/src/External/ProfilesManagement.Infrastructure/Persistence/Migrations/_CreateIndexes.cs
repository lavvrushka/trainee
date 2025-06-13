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
   }

   public override void Down()
   {
         Delete.Index("IX_Patients_Name").OnTable("Patients");
         Delete.Index("IX_Doctors_Name").OnTable("Doctors");
   }
}