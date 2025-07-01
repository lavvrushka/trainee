using AppointmentsManagement.Application.Common.Interfaces.IRepositories;
using AppointmentsManagement.Application.Common.Interfaces.IServices;
using AppointmentsManagement.Infrastructure.Persistense.Configurations;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using AppointmentsManagement.Infrastructure.Persistense.Repositories;
using AppointmentsManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Options;

namespace AppointmentsManagement.API.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(
            configuration.GetSection("ConnectionStrings"));

        services.AddDbContext<AppointmentsManagementDbContext>(
            (serviceProvider, options) =>
            {
                var dbOptions = serviceProvider
                    .GetRequiredService<IOptions<DatabaseOptions>>()
                    .Value;
                options.UseNpgsql(dbOptions.DefaultConnection);
            });

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IResultRepository, ResultRepository>();

        services.AddScoped<IResultPdfService, ResultPdfService>();

        return services;
    }
}
