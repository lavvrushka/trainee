using AppointmentsManagement.Application.UseCases.ResultUseCases;
using AppointmentsManagement.Domain.Models;

namespace AppointmentsManagement.Application.DTOs;

public class ResultDto
{
    public Guid Id { get; set; }
    public string? Complaints { get; set; }
    public string? Conclusion { get; set; }
    public string? Recommendations { get; set; }
    public Guid AppointmentId { get; set; }
}

public static class ResultMapper
{
    public static ResultDto MapToDto(this Result result)
    {
        return new ResultDto
        {
            Id = result.Id,
            Complaints = result.Complaints,
            Conclusion = result.Conclusion,
            Recommendations = result.Recommendations,
            AppointmentId = result.AppointmentId
        };
    }

    public static Result MapToEntity(this CreateResultRequest request)
    {
        return new Result
        {
            Complaints = request.Complaints,
            Conclusion = request.Conclusion,
            Recommendations = request.Recommendations,
            AppointmentId = request.AppointmentId
        };
    }

    public static void MapToEntity(this UpdateResultRequest request, Result result)
    {
        result.Complaints = request.Complaints;
        result.Conclusion = request.Conclusion;
        result.Recommendations = request.Recommendations;
    }
}
