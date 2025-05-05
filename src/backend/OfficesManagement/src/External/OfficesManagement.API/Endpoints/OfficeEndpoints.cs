using OfficesManagement.BuisnessLogic.Common.Interfaces.IServices;
using OfficesManagement.BuisnessLogic.DTOs.Requests;
namespace OfficesManagement.API.Endpoints;

public static class OfficeEndpoints
{
    public static IEndpointRouteBuilder MapOfficeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/offices");

        group.MapGet("/", async (IOfficeService officeService) =>
        {
            var request = new GetAllOfficesRequest(PageIndex: 1, PageSize: 10);
            var result = await officeService.GetAllOfficesAsync(request);

            return Results.Ok(result);
        });

        group.MapGet("/{id:guid}", async (Guid id, IOfficeService officeService) =>
        {
            var request = new GetOfficeByIdRequest(id);
            var officeDto = await officeService.GetOfficeByIdAsync(request);

            return Results.Ok(officeDto);
        });

        group.MapPost("/", async (CreateOfficeRequest request, IOfficeService officeService) =>
        {
            await officeService.CreateOfficeAsync(request);

            return Results.Created($"/api/offices", request);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateOfficeRequest request, IOfficeService officeService) =>
        {
            if (id != request.Id)
            {
                return Results.BadRequest("Идентификатор в URL не соответствует Id в запросе.");
            }
            await officeService.UpdateOfficeAsync(request);

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, IOfficeService officeService) =>
        {
            var request = new DeleteOfficeByIdRequest(id);
            await officeService.DeleteOfficeByIdAsync(request);

            return Results.NoContent();
        });

        group.MapGet("/filter", async (
            string? address,
            string? city,
            string? country,
            string? isActive,
            IOfficeService officeService) =>
        {
            var filterRequest = new FilterOfficesRequest(address, city, country, isActive);
            var offices = await officeService.FilterOfficesAsync(filterRequest);

            return Results.Ok(offices);
        });

        group.MapGet("/active", async (IOfficeService officeService) => 
        { 
            var offices = await officeService.GetActiveOfficesAsync(); 

            return Results.Ok(offices); 
        });

        group.MapGet("/by-name/{name}", async (string name, IOfficeService officeService) =>
        {
            var request = new GetOfficeByNameRequest(name);
            var officeDto = await officeService.GetOfficeByNameAsync(request);

            return Results.Ok(officeDto);
        });

        group.MapGet("/by-city/{city}", async (string city, IOfficeService officeService) =>
        {
            var request = new GetOfficesByCityRequest(city);
            var offices = await officeService.GetOfficesByCityAsync(request);

            return Results.Ok(offices);
        });

        group.MapGet("/by-country/{country}", async (string country, IOfficeService officeService) =>
        {
            var request = new GetOfficesByCountryRequest(country);
            var offices = await officeService.GetOfficesByCountryAsync(request);

            return Results.Ok(offices);
        });

        return group;
    }
}
