using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using ProfilesManagement.Application.Common.Interfaces.IRepositories;
using ProfilesManagement.Application.Common.Interfaces.IServices;
using ProfilesManagement.Infrastructure.Persistence.Factories;
using ProfilesManagement.Infrastructure.Persistence.Migrations;
using ProfilesManagement.Infrastructure.Persistence.Repositories;
using ProfilesManagement.Infrastructure.Services;
using System.Data;

namespace ProfilesManagement.API.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();
        services.AddScoped<IDoctorRepository, DoctorDapperRepository>();
        services.AddScoped<IPatientRepository, PatientDapperRepository>();
        services.AddScoped<IReceptionistRepository, ReceptionistDapperRepository>();
        services.AddScoped<IEmploymentStatusRepository, EmploymentStatusDapperRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationDapperRepository>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddFluentMigratorCore()
                 .ConfigureRunner(rb => rb
                     .AddPostgres()
                     .WithGlobalConnectionString(
                         configuration.GetConnectionString("DefaultConnection"))
                     .ScanIn(typeof(InitialTables).Assembly).For.Migrations()
                 )
                 .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }
    public static IApplicationBuilder UseMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        return app;
    }
}