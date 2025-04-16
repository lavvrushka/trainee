using Microsoft.AspNetCore.Identity;

namespace UserManagement.Domain.Interfaces.Models
{
    public class Role : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}
