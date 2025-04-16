using UserManagement.Domain.Interfaces.Models;
using UserManagement.Application.DTOs;

namespace UserManagement.Application.Common.Mappings
{
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
}
