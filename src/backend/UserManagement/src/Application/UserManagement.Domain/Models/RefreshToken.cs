namespace UserManagement.Domain.Interfaces.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; } = null;
    }
}
