using UserManagement.Application.DTOs.Responses;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.Common.Mappings
{
    public static class TokensPairMapper
    {
        public static UserAuthResponse MapToUserAuthResponse(this TokensPair tokensPair)
        {
            return new UserAuthResponse(tokensPair.AccessToken, tokensPair.RefreshToken);
        }
    }
}
