using OfficesManagement.API.Endpoints;
using OfficesManagement.API.Extensions;
using OfficesManagement.API.Middlewares;
using OfficesManagement.DataAccess.Persistence.Context;
using OfficesManagement.Infrastructure.Persistence.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddBusinessLogic();
builder.Services.AddDataAccessServices();
builder.Services.AddValidationServices();
builder.Services.AddCustomMiddlewares();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("MongoSettings"));
var app = builder.Build();


app.UseCors("AllowReact");

app.UseMiddleware<GlobalExceptionMiddleware>();
OfficeManagementDbContextInitializer.Initialize();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapOfficeEndpoints();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
