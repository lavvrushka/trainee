using OfficesManagement.BuisnessLogic.DTOs.Requests;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.DTOs.Requests;
using OfficesManagement.Core.DTOs.Responses;
using OfficesManagement.Core.Models;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Models.ValueObjects;


namespace OfficesManagement.Core.Mapper;

public static class MappTo
{
    public static Location MapToLocation(this LocationRequest request)
    {
        return new Location
        {
            Country = request.Country,
            City = request.City,
            Address = request.Address
        };
    }

    public static Office MapToOffice(this CreateOfficeRequest request)
    {
        return new Office
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsActive = request.IsActive,
            RegistryPhoneNumber = request.RegistryPhoneNumber,
            Location = request.Location.MapToLocation()
        };
    }

    public static OfficeDto MapToOfficeDto(this Office office)
    {
        return new OfficeDto(
            Id: office.Id,
            Name: office.Name,
            location: new LocationResponse(
                Address: office.Location.Address,
                City: office.Location.City,
                Country: office.Location.Country
            ),
            IsActive: office.IsActive,
            RegistryPhoneNumber: office.RegistryPhoneNumber
        );
    }

    public static void MapToOffice(this UpdateOfficeRequest request, Office office)
    {
        office.Name = request.Name;
        office.RegistryPhoneNumber = request.RegistryPhoneNumber;
        office.IsActive = request.IsActive;

        if (request.Location is not null)
        {
            office.Location = request.Location.MapToLocation();
        }
    }
    public static PageSettings MapToPageSettings(this GetAllOfficesRequest request)
    {
        return new PageSettings
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

}
