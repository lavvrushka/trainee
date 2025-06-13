namespace ProfilesManagement.Domain.Models;

public class Doctor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public Guid AccountId { get; set; }
    public Guid OfficeId { get; set; }
    public Guid SpecializationId { get; set; }
    public Specialization Specialization { get; set; } = null!;
    public DateTime CareerStartYear { get; set; }
    public Guid StatusId { get; set; }
    public EmploymentStatus Status { get; set; } = null!;
    public Guid? ImageId { get; set; }
}
