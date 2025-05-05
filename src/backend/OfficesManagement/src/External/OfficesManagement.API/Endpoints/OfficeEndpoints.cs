using MediatR;
using OfficesManagement.Core.DTOs.Requests; // Содержит CreateOfficeRequest
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using Microsoft.AspNetCore.Builder;
using OfficesManagement.Core.UseCases;

namespace OfficesManagement.API.Endpoints
{
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

            // Используем MediatR для создания офиса
            group.MapPost("/", async (CreateOfficeRequest request, IMediator mediator) =>
            {
                // Отправляем команду на создание офиса
                await mediator.Send(request);
                // Можно вернуть LocationHeader с новым ресурсом, если в ответе нет Id
                return Results.Created($"/api/offices", request);
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
}
