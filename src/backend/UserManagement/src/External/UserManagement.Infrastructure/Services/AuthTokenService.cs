using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Infrastructure.Services
{
    public class AuthTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AuthTokenService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Account> userManager,
            RoleManager<Role> roleManager)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateAccessToken(Account account)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

            var roles = await _userManager.GetRolesAsync(account);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new(ClaimTypes.Email, account.Email),
                new(ClaimTypes.Role, roles.FirstOrDefault() ?? "User")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var refreshToken = new RefreshToken
            {
                AccountId = userId, 
                Token = Guid.NewGuid().ToString("N"),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationDays"]))
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            return refreshToken.Token;
        }

        public async Task RevokeRefreshTokenAsync()
        {
            var token = ExtractTokenFromHeader() ?? throw new UnauthorizedAccessException("Token is missing.");
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            if (refreshToken != null)
            {
                await _unitOfWork.RefreshTokens.DeleteAsync(refreshToken);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<(string newAccessToken, string RefreshToken)> RefreshTokensAsync(string token)
        {
            var oldToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            if (oldToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (oldToken.Expires < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired.");

            var user = await _userManager.FindByIdAsync(oldToken.AccountId.ToString());
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var newAccessToken = await GenerateAccessToken(user);

            return (newAccessToken, oldToken.Token);
        }

        public string? ExtractTokenFromHeader()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
                return null;

            return authorizationHeader.StartsWith("Bearer ")
                ? authorizationHeader.Substring("Bearer ".Length).Trim()
                : null;
        }

        public Guid? ExtractUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                    throw new UnauthorizedAccessException("Invalid token format.");

                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    throw new UnauthorizedAccessException("UserId not found in token.");

                return Guid.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Token validation failed.", ex);
            }
        }

        public async Task<Account> AuthenticateUserAsync(string token)
        {
            var userId = ExtractUserIdFromToken(token)
                ?? throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new UnauthorizedAccessException("User not found.");

            return user;
        }
    }
}
