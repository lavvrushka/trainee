namespace ProfilesManagement.Domain.Models;

public class Specialization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}