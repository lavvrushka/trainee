namespace UserManagement.Application.DTOs.Responses
{
    public record UserAuthResponse(
       string AccessToken,
       string RefreshToken
   );
}
