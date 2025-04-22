using MediatR;
using UserManagement.Application.Common.Behaviors;
using UserManagement.Application.UseCases.AccountTools;
using UserManagement.Application.UseCases.Auth;
using UserManagement.Application.UseCases.AuthUsecases;

namespace UserManagement.API.Extensions
{
    public static class ApplicationCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(UserLoginRequest).Assembly);
            });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddScoped<LoginHandler>();
            services.AddScoped<LogoutHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<ConfirmEmailHandler>();
            services.AddScoped<SendPasswordTokenHandler>();
            services.AddScoped<SetNewPasswordHandler>();
            services.AddScoped<CurrentUserHandler>();

            return services;
        }
    }
}
