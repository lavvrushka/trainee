using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.Interfaces.Models
{
    public class Patient:BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
    }
}
