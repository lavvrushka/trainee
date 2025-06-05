using OfficesManagement.Core.DTOs.Requests;
namespace OfficesManagement.BuisnessLogic.DTOs.Requests;

public record UpdateOfficeRequest(
    Guid Id,
    string Name,
    LocationRequest? Location,
    bool IsActive,
    string RegistryPhoneNumber
);
