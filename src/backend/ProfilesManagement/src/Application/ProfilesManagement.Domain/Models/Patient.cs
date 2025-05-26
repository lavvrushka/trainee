namespace ProfilesManagement.Domain.Models;

public class Patient
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public Guid AccountId { get; set; }
    public Guid? ImageId { get; set; } = null;
    public Image? Image { get; set; } = null;
}
