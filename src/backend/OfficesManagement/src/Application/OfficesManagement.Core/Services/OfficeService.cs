using System.Linq.Expressions;
using OfficesManagement.BuisnessLogic.Common.Interfaces.IServices;
using OfficesManagement.Core.Common.Exceptions;
using OfficesManagement.Core.Common.Interfaces.IRepositories;
using OfficesManagement.Core.DTOs;
using OfficesManagement.BuisnessLogic.DTOs.Requests;
using OfficesManagement.Core.Mapper;
using OfficesManagement.Core.Models.Entities;
using OfficesManagement.Core.Models;

namespace OfficesManagement.BuisnessLogic.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;

        public OfficeService(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task CreateOfficeAsync(CreateOfficeRequest request)
        {
            var officeEntity = request.MapToOffice();
            await _officeRepository.AddAsync(officeEntity);
        }

        public async Task DeleteOfficeByIdAsync(DeleteOfficeByIdRequest request)
        {
            var office = await _officeRepository.GetByIdAsync(request.Id);

            if (office is null)
            {
                throw new NotFoundException($"Office with Id = {request.Id} was not found.");
            }
            await _officeRepository.DeleteAsync(office);
        }

        public async Task<List<OfficeDto>> FilterOfficesAsync(FilterOfficesRequest request)
        {
            Expression<Func<Office, bool>> filter = o =>
                (string.IsNullOrWhiteSpace(request.Address) || o.Location.Address.Contains(request.Address!)) &&
                (string.IsNullOrWhiteSpace(request.City) || o.Location.City.Equals(request.City!)) &&
                (string.IsNullOrWhiteSpace(request.Country) || o.Location.Country.Equals(request.Country!)) &&
                (string.IsNullOrWhiteSpace(request.IsActive) || o.IsActive.ToString().Equals(request.IsActive!, StringComparison.OrdinalIgnoreCase));

            var offices = await _officeRepository.GetFilteredAsync(filter);

            return offices.Select(o => o.MapToOfficeDto()).ToList();
        }

        public async Task<List<OfficeDto>> GetActiveOfficesAsync()
        {
            var offices = await _officeRepository.GetActiveOfficesAsync();

            return offices.Select(o => o.MapToOfficeDto()).ToList();
        }

        public async Task<Pagination<OfficeDto>> GetAllOfficesAsync(GetAllOfficesRequest request)
        {
            var pageSettings = request.MapToPageSettings();
            var offices = await _officeRepository.GetPageAsync(pageSettings);
            var totalCount = await _officeRepository.GetOfficeCountAsync();
            var officeDtos = offices.Select(o => o.MapToOfficeDto()).ToList();

            return new Pagination<OfficeDto>(officeDtos, totalCount, pageSettings);
        }

        public async Task<OfficeDto> GetOfficeByIdAsync(GetOfficeByIdRequest request)
        {
            var office = await _officeRepository.GetByIdAsync(request.Id);

            if (office is null)
            {
                throw new NotFoundException($"Office with Id = {request.Id} was not found.");
            }

            return office.MapToOfficeDto();
        }

        public async Task<OfficeDto> GetOfficeByNameAsync(GetOfficeByNameRequest request)
        {
            var office = await _officeRepository.GetOfficeByNameAsync(request.Name);

            if (office is null)
            {
                throw new NotFoundException($"Office with Name = '{request.Name}' was not found.");
            }

            return office.MapToOfficeDto();
        }

        public async Task<List<OfficeDto>> GetOfficesByCityAsync(GetOfficesByCityRequest request)
        {
            var offices = await _officeRepository.GetOfficesByCityAsync(request.City);

            return offices.Select(o => o.MapToOfficeDto()).ToList();
        }

        public async Task<List<OfficeDto>> GetOfficesByCountryAsync(GetOfficesByCountryRequest request)
        {
            var offices = await _officeRepository.GetOfficesByCountryAsync(request.Country);

            return offices.Select(o => o.MapToOfficeDto()).ToList();
        }

        public async Task UpdateOfficeAsync(UpdateOfficeRequest request)
        {
            var office = await _officeRepository.GetByIdAsync(request.Id);

            if (office is null)
            {
                throw new NotFoundException($"Office with Id = {request.Id} not found");
            }

            request.MapToOffice(office);

            await _officeRepository.UpdateAsync(office);
        }
    }
}
