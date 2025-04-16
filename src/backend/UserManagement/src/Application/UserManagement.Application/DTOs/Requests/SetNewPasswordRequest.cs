using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record SetNewPasswordRequest(
    Guid UserId,
    string Token,
    string NewPassword,
    string ConfirmPassword
) : IRequest<Unit>;

}
