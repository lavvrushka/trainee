using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record CurrentUserRequest() : IRequest<UserDto>;
}
