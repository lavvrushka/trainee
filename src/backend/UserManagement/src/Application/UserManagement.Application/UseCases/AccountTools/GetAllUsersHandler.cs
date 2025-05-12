using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Interfaces.Models;

namespace UserManagement.Application.UseCases.UserUsecases;

public record GetAllUsersRequest(int PageNumber, int PageSize) : IRequest<(List<UserDto> Users, int TotalCount)>;

public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, (List<UserDto> Users, int TotalCount)>
{
    private readonly UserManager<Account> _userManager;

    public GetAllUsersHandler(UserManager<Account> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(List<UserDto> Users, int TotalCount)> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
    {
        var totalCount = await _userManager.Users.CountAsync(cancellationToken);

        var users = await _userManager.Users
            .Skip((request.PageNumber - 1) * request.PageSize) 
            .Take(request.PageSize) 
            .ToListAsync(cancellationToken);

        if (users == null || !users.Any())
        {
            throw new Exception("No users found.");
        }

        var userDtos = users.Select(u => u.MapToUserDto()).ToList();

        return (userDtos, totalCount);
    }
}
