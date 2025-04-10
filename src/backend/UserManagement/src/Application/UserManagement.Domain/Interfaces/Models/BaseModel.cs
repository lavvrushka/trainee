using System.ComponentModel.DataAnnotations;

namespace UserManagement.Domain.Interfaces.Models
{
    public abstract class BaseModel
    {
        public Guid Id { get; set; }
    }
}
