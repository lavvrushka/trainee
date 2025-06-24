using AppointmentsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace AppointmentsManagement.Infrastructure.Persistense.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        
        builder.ToTable("Appointments");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
               .ValueGeneratedOnAdd();

        builder.Property(a => a.PatientId)
               .IsRequired();
        builder.Property(a => a.DoctorId)
               .IsRequired();
        builder.Property(a => a.ServiceId)
               .IsRequired();

        builder.Property(a => a.Date)
               .IsRequired()
               .HasColumnType("datetime")    
               .HasComment("Дата и время приёма");

        builder.Property(a => a.IsApproved)
               .IsRequired()
               .HasDefaultValue(false)
               .HasComment("Статус подтверждения");

        builder.HasIndex(a => a.PatientId)
               .HasDatabaseName("IX_Appointments_Patient");
        builder.HasIndex(a => new { a.DoctorId, a.Date })
               .HasDatabaseName("IX_Appointments_Doctor_Date");

        builder.HasOne(a => a.Result)
               .WithOne(r => r.Appointment)
               .HasForeignKey<Result>(r => r.AppointmentId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}