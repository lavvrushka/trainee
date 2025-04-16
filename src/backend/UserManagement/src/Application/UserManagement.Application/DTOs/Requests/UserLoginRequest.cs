using MediatR;
using UserManagement.Application.DTOs.Responses;

namespace UserManagement.Application.DTOs.Requests
{
    public record UserLoginRequest(
      string Email,
      string Password
  ) : IRequest<UserAuthResponse>;
}
