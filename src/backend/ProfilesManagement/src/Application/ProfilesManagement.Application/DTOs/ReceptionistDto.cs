using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.DTOs;

public class ReceptionistDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public EmploymentStatus Status { get; set; }
    public Guid AccountId { get; set; }
    public Guid? OfficeId { get; set; }
    public Guid? ImageId { get; set; }
}


public static class ReceptionistMapper
{
    public static ReceptionistDto MapToReceptionistDto(this Receptionist receptionist)
    {
        return new ReceptionistDto
        {
            Id = receptionist.Id,
            FirstName = receptionist.FirstName,
            LastName = receptionist.LastName,
            MiddleName = receptionist.MiddleName,
            Status = receptionist.Status,
            AccountId = receptionist.AccountId,
            OfficeId = receptionist.OfficeId,
            ImageId = receptionist.ImageId
        };
    }

    public static Receptionist MapToReceptionist(this CreateReceptionistRequest request)
    {
        return new Receptionist
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            Status = request.Status,
            AccountId = request.AccountId,
            OfficeId = request.OfficeId,
            ImageId = request.ImageId
        };
    }

    public static void MapToReceptionist(this UpdateReceptionistRequest request, Receptionist receptionist)
    {
        receptionist.FirstName = request.FirstName;
        receptionist.LastName = request.LastName;
        receptionist.MiddleName = request.MiddleName;
        receptionist.Status = request.Status;
        receptionist.AccountId = request.AccountId;
        receptionist.OfficeId = request.OfficeId;
        receptionist.ImageId = request.ImageId;
    }
}
