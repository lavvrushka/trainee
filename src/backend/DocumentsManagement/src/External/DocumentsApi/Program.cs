using BackgroundJobs.Services;
using DocumentsAPI.Extensions;
using DocumentsAPI.Middlewares;
using DocumentsDataAccess.Persistence.Context;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddCustomMiddlewares();
builder.Services.AddBlobServices(builder.Configuration);

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
    var factory = new ConnectionFactory
    {
        HostName = cfg["HostName"],
        UserName = cfg["UserName"],
        Password = cfg["Password"]
    };
    return factory.CreateConnection();
});

builder.Services.AddHostedService<DocumentCreatedConsumer>();
builder.Services.AddHostedService<PurgeOldEntitiesService>();
builder.Services.AddHostedService<HardDeleteService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

await AppContextInitializer.InitializeAsync(app.Services);

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