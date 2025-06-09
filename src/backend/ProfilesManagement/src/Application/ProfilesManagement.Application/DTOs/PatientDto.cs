using ProfilesManagement.Application.UseCases.Patient;
using ProfilesManagement.Domain.Models;

namespace ProfilesManagement.Application.DTOs;
public class PatientDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public Guid AccountId { get; set; }
    public Guid? ImageId { get; set; }
}

public static class PatientMapper
{
    public static PatientDto MapToPatientDto(this Patient patient)
    {
        return new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            MiddleName = patient.MiddleName,
            AccountId = patient.AccountId,
            ImageId = patient.ImageId
        };
    }
    public static Patient MapToPatient(this CreatePatientRequest request)
    {
        return new Patient
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            AccountId = request.AccountId,
            ImageId = request.ImageId
        };
    }
    public static void MapToPatient(this UpdatePatientRequest request, Patient patient)
    {
        patient.FirstName = request.FirstName;
        patient.LastName = request.LastName;
        patient.MiddleName = request.MiddleName;
        patient.ImageId = request.ImageId;
    }
}