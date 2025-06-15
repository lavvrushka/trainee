using Microsoft.AspNetCore.Http;
using ProfilesManagement.Application.Common.Interfaces.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace ProfilesManagement.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserIdFromAccessToken()
    {
        var token = ExtractTokenFromHeader();

        if (token is null)
        {
            throw new UnauthorizedAccessException("Token is missing.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken is null)
        {
            throw new UnauthorizedAccessException("Invalid token.");
        }

        var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
            c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");

        if (userIdClaim is null)
        {
            throw new UnauthorizedAccessException("UserId not found in token.");
        }

        return Guid.Parse(userIdClaim.Value);
    }

    private string? ExtractTokenFromHeader()
    {
        var authorizationHeader = _httpContextAccessor.HttpContext?
            .Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return null;
        }

        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authorizationHeader["Bearer ".Length..].Trim();
        }

        return null;
    }
}
