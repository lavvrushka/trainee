using UserManagement.Domain.Interfaces.Models;
namespace UserManagement.Application.DTOs.Responses;

public record UserAuthResponse(
   string AccessToken,
   string RefreshToken
);

public static class TokensPairMapper
{
    public static UserAuthResponse MapToUserAuthResponse(this TokensPair tokensPair)
    {
        return new UserAuthResponse(tokensPair.AccessToken, tokensPair.RefreshToken);
    }
}
