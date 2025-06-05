using OfficesManagement.Core.DTOs.Responses;
namespace OfficesManagement.Core.DTOs;

public record OfficeDto
    (
        Guid Id,
        string Name,
        LocationResponse location,
        bool IsActive,
        string RegistryPhoneNumber
    );