using ProfilesManagement.Application.UseCases.DoctorUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.DTOs;

public class DoctorDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;

    public Guid AccountId { get; set; }
    public Guid OfficeId { get; set; }
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;

    public DateTime CareerStartYear { get; set; }

    public Guid StatusId { get; set; }
    public EmploymentStatus Status { get; set; } = null!;

    public Guid? ImageId { get; set; }
}

public static class DoctorMapper
{
    public static DoctorDto MapToDoctorDto(this Doctor doctor)
    {
        return new DoctorDto
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            MiddleName = doctor.MiddleName,
            AccountId = doctor.AccountId,
            OfficeId = doctor.OfficeId,
            SpecializationId = doctor.SpecializationId,
            Specialization = doctor.Specialization,
            CareerStartYear = doctor.CareerStartYear,
            StatusId = doctor.StatusId,
            Status = doctor.Status,
            ImageId = doctor.ImageId
        };
    }

    public static Doctor MapToDoctor(this CreateDoctorRequest request)
    {
        return new Doctor
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            AccountId = request.AccountId,
            OfficeId = request.OfficeId,
            SpecializationId = request.SpecializationId,
            CareerStartYear = request.CareerStartYear,
            StatusId = request.StatusId,
            ImageId = request.ImageId
        };
    }

    public static void MapToDoctor(this UpdateDoctorRequest request, Doctor doctor)
    {
        doctor.FirstName = request.FirstName;
        doctor.LastName = request.LastName;
        doctor.MiddleName = request.MiddleName;
        doctor.AccountId = request.AccountId;
        doctor.OfficeId = request.OfficeId;
        doctor.SpecializationId = request.SpecializationId;
        doctor.CareerStartYear = request.CareerStartYear;
        doctor.StatusId = request.StatusId;
        doctor.ImageId = request.ImageId;
    }
}
