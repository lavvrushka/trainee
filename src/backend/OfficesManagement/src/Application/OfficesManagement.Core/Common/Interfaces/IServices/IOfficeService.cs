using OfficesManagement.BuisnessLogic.DTOs.Requests;
using OfficesManagement.Core.DTOs;
using OfficesManagement.Core.Models;
namespace OfficesManagement.BuisnessLogic.Common.Interfaces.IServices;

public interface IOfficeService
{
    Task CreateOfficeAsync(CreateOfficeRequest request);
    Task DeleteOfficeByIdAsync(DeleteOfficeByIdRequest request);
    Task<List<OfficeDto>> FilterOfficesAsync(FilterOfficesRequest request);
    Task<List<OfficeDto>> GetActiveOfficesAsync(); 
    Task<Pagination<OfficeDto>> GetAllOfficesAsync(GetAllOfficesRequest request);
    Task<OfficeDto> GetOfficeByIdAsync(GetOfficeByIdRequest request);
    Task<OfficeDto> GetOfficeByNameAsync(GetOfficeByNameRequest request);
    Task<List<OfficeDto>> GetOfficesByCityAsync(GetOfficesByCityRequest request);
    Task<List<OfficeDto>> GetOfficesByCountryAsync(GetOfficesByCountryRequest request);
    Task UpdateOfficeAsync(UpdateOfficeRequest request);
}
