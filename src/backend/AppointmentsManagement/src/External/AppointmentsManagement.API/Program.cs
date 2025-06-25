using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddFastEndpoints();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
