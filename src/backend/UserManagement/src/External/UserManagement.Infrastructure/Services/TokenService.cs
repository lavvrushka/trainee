using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Domain.Interfaces.IRepositories;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Infrastructure.Persistence.Configurations;
namespace UserManagement.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<Account> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public TokenService(IOptions<JwtSettings> jwtSettings,
                        IHttpContextAccessor httpContextAccessor,
                        UserManager<Account> userManager,
                        IRefreshTokenRepository refreshTokenRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<TokensPair> GenerateTokensPairAsync(Account user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var refreshToken = new RefreshToken
        {
            AccountId = user.Id,
            Token = Guid.NewGuid().ToString("N"),
            Created = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new TokensPair
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = refreshToken.Token
        };
    }

    public async Task<TokensPair> RefreshTokensAsync(string refreshTokenValue)
    {
        var userId = GetUserIdFromAccessToken();

        var oldToken = await _refreshTokenRepository.GetByTokenAndUserIdAsync(refreshTokenValue, userId);

        if (oldToken is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        var user = await _userManager.FindByIdAsync(oldToken.AccountId.ToString());

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        await _refreshTokenRepository.DeleteAsync(oldToken);
        await _refreshTokenRepository.SaveChangesAsync();

        var newTokensPair = await GenerateTokensPairAsync(user);
        return newTokensPair;
    }

    public async Task RevokeRefreshTokenAsync()
    {
        var refreshTokenValue = ExtractTokenFromHeader();

        if (refreshTokenValue is null)
        {
            throw new UnauthorizedAccessException("Token is missing.");
        }

        var userId = GetUserIdFromAccessToken();
        var refreshToken = await _refreshTokenRepository.GetByTokenAndUserIdAsync(refreshTokenValue, userId);

        if (refreshToken is null)
        {
            return;
        }

        await _refreshTokenRepository.DeleteAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();
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
