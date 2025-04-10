using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.Interfaces.Models
{
    public class Doctor:BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int CareerStartYear { get; set; }
        public string Specialization { get; set; }

        public Guid? OfficeId { get; set; }

        public Guid AccountId { get; set; }
        public Account Account { get; set; }

    }
}
