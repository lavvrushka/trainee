using MediatR;

namespace UserManagement.Application.DTOs.Requests
{
    public record GetAllUsersRequest() : IRequest<List<UserDto>>;
}
