using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record UpdateUserRequest( string Email) : IRequest<Unit>;
}
