using ProfilesManagement.Application.UseCases.DoctorUseCases;
using ProfilesManagement.Application.UseCases.PatientUseCases;
using ProfilesManagement.Application.UseCases.ReceptionistUseCases;
using ProfilesManagement.Domain.Models;
namespace ProfilesManagement.Application.DTOs;

public static class PaginationMapper
{
    public static Pagination<PatientDto> MapToPatientDtoPage(this Pagination<Patient> sourcePage)
    {
        var dtoItems = sourcePage.Items.Select(patient => patient.MapToPatientDto()).ToList();

        var pageSettings = new PageSettings
        {
            PageIndex = sourcePage.CurrentPage,
            PageSize = sourcePage.PageSize
        };

        return new Pagination<PatientDto>(dtoItems,sourcePage.TotalCount,pageSettings);
    }
    public static Pagination<DoctorDto> MapToDoctorDtoPage(this Pagination<Doctor> sourcePage)
    {
        var dtoItems = sourcePage.Items.Select(doctor => doctor.MapToDoctorDto()).ToList();

        var pageSettings = new PageSettings
        {
            PageIndex = sourcePage.CurrentPage,
            PageSize = sourcePage.PageSize
        };

        return new Pagination<DoctorDto>(dtoItems,sourcePage.TotalCount,pageSettings);
    }
    public static Pagination<ReceptionistDto> MapToReceptionistDtoPage(this Pagination<Receptionist> sourcePage)
    {
        var dtoItems = sourcePage.Items.Select(receptionist => receptionist.MapToReceptionistDto()).ToList();

        var pagesettings = new PageSettings
        {
            PageIndex = sourcePage.CurrentPage,
            PageSize = sourcePage.PageSize
        };

        return new Pagination<ReceptionistDto>(dtoItems, sourcePage.TotalCount, pagesettings);
    }

    public static PageSettings MapToPageSettings(this GetAllPatientsRequest request)
    {
        return new PageSettings
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
    public static PageSettings MapToPageSettings(this GetAllDoctorsRequest request)
    {
        return new PageSettings
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
    public static PageSettings MapToPageSettings(this GetAllReceptionistsRequest request)
    {
        return new PageSettings
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}
