// файл: Application/DTOs/PaginationMapper.cs
using System.Linq;
using ProfilesManagement.Domain.Models;      // Pagination<T>, PageSettings, Patient
using ProfilesManagement.Application.DTOs;  // PatientDto, PatientMapper

namespace ProfilesManagement.Application.DTOs
{
    public static class PaginationMapper
    {
        // Это расширение привязано строго к Pagination<Domain.Models.Patient>
        public static Pagination<PatientDto> MapToPatientDtoPage(
            this Pagination<Patient> sourcePage)
        {
            var dtoItems = sourcePage.Items
                                     .Select(p => p.MapToPatientDto())
                                     .ToList();

            var pageSettings = new PageSettings
            {
                PageIndex = sourcePage.CurrentPage,
                PageSize = sourcePage.PageSize
            };

            return new Pagination<PatientDto>(
                items: dtoItems,
                count: sourcePage.TotalCount,
                pageSettings: pageSettings
            );
        }
    }
}
