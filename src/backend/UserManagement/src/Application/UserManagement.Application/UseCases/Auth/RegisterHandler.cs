using MediatR;
using Microsoft.AspNetCore.Identity;
using UserManagement.Application.DTOs.Requests;
using UserManagement.Application.DTOs.Responses;
using UserManagement.Domain.Interfaces.IServices;
using UserManagement.Domain.Interfaces.Models;
using UserManagement.Application.Common.Mappings;

namespace UserManagement.Application.UseCases.AuthUsecases
{
    public class RegisterUserHandler : IRequestHandler<UserRegisterRequest, UserAuthResponse>
    {
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailConfirmationService _emailConfirmationService;

        public RegisterUserHandler(UserManager<Account> userManager,
                                   RoleManager<Role> roleManager,
                                   ITokenService tokenService,
                                   IEmailConfirmationService emailConfirmationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailConfirmationService = emailConfirmationService;
        }

        public async Task<UserAuthResponse> Handle(UserRegisterRequest request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Пользователь с таким email уже зарегистрирован.");
            }

            var newUser = new Account
            {
                Email = request.Email,
                UserName = request.Email,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.Email
            };

            var createResult = await _userManager.CreateAsync(newUser, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new Exception($"Ошибка создания пользователя: {errors}");
            }

            const string defaultRole = "PATIENT";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                throw new Exception($"Роль {defaultRole} не существует в системе.");
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, defaultRole);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Ошибка назначения роли: {errors}");
            }
            var newConfirmationToken = await _emailConfirmationService.GenerateEmailConfirmationTokenAsync(newUser.Id);
            newUser.EmailConfirmationToken = newConfirmationToken;
            await _emailConfirmationService.SendConfirmationEmailAsync(newUser.Id);

            var tokensPair = await _tokenService.GenerateTokensPairAsync(newUser);

            return tokensPair.MapToUserAuthResponse();
        }
    }
}
