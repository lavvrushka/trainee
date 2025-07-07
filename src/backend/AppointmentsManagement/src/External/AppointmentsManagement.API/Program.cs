using AppointmentsManagement.API.Extensions;
using AppointmentsManagement.API.Middlewares;
using AppointmentsManagement.Application.Common.Interfaces.IServices;
using AppointmentsManagement.Infrastructure.Messaging;
using AppointmentsManagement.Infrastructure.Persistense.Context;
using AppointmentsManagement.Infrastructure.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Connections;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCustomMiddlewares();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = ": 'Bearer abcdef12345'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddSingleton(sp =>
{
    var cfg = builder.Configuration.GetSection("RabbitMq");
    return new ConnectionFactory
    {
        HostName = cfg["HostName"],
        UserName = cfg["UserName"],
        Password = cfg["Password"]
    }.CreateConnectionAsync();
});
builder.Services.AddSingleton<IMessagingPublisher, RabbitMqPublisher>();


var app = builder.Build();
app.UseFastEndpoints();
app.UseMiddleware<GlobalExceptionMiddleware>();


await AppointmentsManagementDbContextInitializer.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
