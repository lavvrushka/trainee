using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement.Application.DTOs;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Application.Common.Mappings;

namespace UserManagement.Application.UseCases.UserUsecases
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, List<UserDto>>
    {
        private readonly UserManager<Account> _userManager;

        public GetAllUsersHandler(UserManager<Account> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);
            if (users == null || !users.Any())
            {
                throw new Exception("No users found.");
            }

            var userDtos = users.Select(u => u.MapToUserDto()).ToList();
            return userDtos;
        }
    }
}
