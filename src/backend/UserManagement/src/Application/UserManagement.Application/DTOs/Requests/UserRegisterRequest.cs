using UserManagement.Application.DTOs.Responses;
using MediatR;
namespace UserManagement.Application.DTOs.Requests
{
    public record UserRegisterRequest
  (
       string Email,
       string Password,
       string ConfirmPassword
  ) : IRequest<UserAuthResponse>;
}
