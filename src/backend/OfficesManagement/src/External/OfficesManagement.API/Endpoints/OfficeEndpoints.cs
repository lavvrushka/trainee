using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
namespace OfficesManagement.API.Endpoints;
public static class OfficeEndpoints
{
    public static IEndpointRouteBuilder MapOfficeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/offices"); 

        group.MapGet("/", async (IOfficeRepository repo) =>
        {
            var offices = await repo.GetAllAsync();
            return Results.Ok(offices);
        });

        group.MapGet("/{id:guid}", async (Guid id, IOfficeRepository repo) =>
        {
            var office = await repo.GetByIdAsync(id);
            return office is not null ? Results.Ok(office) : Results.NotFound();
        });

        group.MapPost("/", async (Office office, IOfficeRepository repo) =>
        {
            await repo.AddAsync(office);
            return Results.Created($"/api/offices/{office.Id}", office);
        });

        group.MapPut("/{id:guid}", async (Guid id, Office office, IOfficeRepository repo) =>
        {
            if (id != office.Id)
            {
                return Results.BadRequest("Идентификатор не соответствует");
            }
            await repo.UpdateAsync(office);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, IOfficeRepository repo) =>
        {
            var office = await repo.GetByIdAsync(id);
            if (office is null)
            {
                return Results.NotFound();
            }
            await repo.DeleteAsync(office);
            return Results.NoContent();
        });

        return routes;
    }
}
