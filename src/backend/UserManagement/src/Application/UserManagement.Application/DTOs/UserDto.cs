using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime EmailVerifiedAt { get; set; }
}

public static class AccountMapper
{
    public static UserDto MapToUserDto(this Account account)
    {
        return new UserDto
        {
            Id = account.Id,
            Email = account.Email,
            BirthDate = account.BirthDate,
            EmailVerifiedAt = account.EmailVerifiedAt ?? DateTime.MinValue
        };
    }
}
