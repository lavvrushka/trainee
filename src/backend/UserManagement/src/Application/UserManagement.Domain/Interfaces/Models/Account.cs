using Microsoft.AspNetCore.Identity;

namespace UserManagement.Domain.Interfaces.Models
{
    public class Account : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string UpdatedBy { get; set; }

        public DateTime BirthDate { get; set; }

        public Guid? PhotoId { get; set; }

        public Guid RoleId { get; set; }
        public Role? Role { get; set; }

        public DateTime? EmailVerifiedAt { get; set; }
        public string? EmailConfirmationToken { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        public string? AccountRecoveryToken { get; set; }
        public DateTime? AccountRecoveryTokenExpires { get; set; }
    }
}
