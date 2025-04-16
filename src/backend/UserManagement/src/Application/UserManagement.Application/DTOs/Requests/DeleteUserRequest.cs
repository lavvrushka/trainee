using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record DeleteUserRequest(Guid Id) : IRequest<Unit>;
}
