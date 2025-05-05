using OfficesManagement.Core.DTOs.Requests;
namespace OfficesManagement.BuisnessLogic.DTOs.Requests;

public record CreateOfficeRequest(
     string Name,
     LocationRequest Location,
     bool IsActive,
     string RegistryPhoneNumber
 );
