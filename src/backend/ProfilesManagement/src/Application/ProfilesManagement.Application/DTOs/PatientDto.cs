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
}