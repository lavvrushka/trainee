using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record SendPasswordTokenRequest(string email) : IRequest<Unit>;
}
