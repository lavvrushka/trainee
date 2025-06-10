using ProfilesManagement.Application.UseCases.EmploymentStatusUseCases;
using ProfilesManagement.Application.UseCases.PatientUseCases;
using ProfilesManagement.Domain.Models;

namespace ProfilesManagement.Application.DTOs;

public class EmploymentStatusDto
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime DateTime { get; set; }
}

public static class EmploymentMapper
{
    public static EmploymentStatusDto MapEmploymentStatusToDto(this EmploymentStatus employmentStatus)
    {
        return new EmploymentStatusDto
        {
            Id = employmentStatus.Id,
            Status = employmentStatus.Status,
            Description = employmentStatus.Description,
            DateTime = employmentStatus.DateTime
        };
    }
  
    public static EmploymentStatus MapEmploymentStatusDomain(this CreateEmploymentStatusRequest request)
    {
        return new EmploymentStatus
        {
            Id = Guid.NewGuid(),
            Status = request.Status,
            Description = request.Description,
            DateTime = request.DateTime
        };
    }
    public static void MapEmploymentStatusDomain(this UpdateEmploymentStatusRequest request, EmploymentStatus employmentStatus)
    {
        employmentStatus.Status = request.Status;
        employmentStatus.Description = request.Description;
        employmentStatus.DateTime = request.DateTime;
    }

}