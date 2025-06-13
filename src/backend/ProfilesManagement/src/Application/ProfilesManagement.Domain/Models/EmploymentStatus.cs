namespace ProfilesManagement.Domain.Models;

public class EmploymentStatus
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}