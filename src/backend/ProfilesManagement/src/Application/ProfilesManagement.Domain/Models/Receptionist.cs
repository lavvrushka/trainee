using ProfilesManagement.Domain.Enums;
namespace ProfilesManagement.Domain.Models;

public class Receptionist
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public EmploymentStatus Status { get; set; }
    public Guid AccountId { get; set; }
    public Guid ? OfficeId { get; set; } = null;
    public Guid? ImageId { get; set; } = null;
    public Image? Image { get; set; } = null;
}
