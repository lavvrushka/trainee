namespace UserManagement.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime EmailVerifiedAt { get; set; }
    }
}
