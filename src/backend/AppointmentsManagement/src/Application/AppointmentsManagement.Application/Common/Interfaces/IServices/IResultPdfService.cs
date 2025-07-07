using AppointmentsManagement.Application.DTOs;

namespace AppointmentsManagement.Application.Common.Interfaces.IServices;

public interface IResultPdfService
{
    byte[] GenerateResultPdf(AppointmentDto appointment);
}