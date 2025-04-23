using OfficesManagement.Core.Models.ValueObjects;
namespace OfficesManagement.Core.Models.Entities;
public class Office
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Location Location { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public string RegistryPhoneNumber { get; set; }
}