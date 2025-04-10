namespace UserManagement.Domain.Interfaces.Models
{
    public class Role:BaseModel
    {
        public string Name { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }

}
