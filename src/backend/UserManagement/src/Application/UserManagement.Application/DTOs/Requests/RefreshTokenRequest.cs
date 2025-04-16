using MediatR;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.DTOs.Requests
{
    public record RefreshTokenRequest(string RefreshToken) : IRequest<UserAuthResponse>;
}
