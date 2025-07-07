using AppointmentsManagement.Application.Common.Interfaces.IServices;
using AppointmentsManagement.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AppointmentsManagement.Infrastructure.Services;

public class ResultPdfService : IResultPdfService
{
    public byte[] GenerateResultPdf(AppointmentDto appointment)
    {
        var result = appointment.Result ?? new ResultDto();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Column(col =>
                    {
                        col.Spacing(2);

                        col.Item().Text($"Результат приёма №{result.Id}")
                            .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                        col.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Medium)
                            .Background(Colors.Grey.Lighten3)
                            .Padding(10)
                            .Row(row =>
                            {
                                row.ConstantItem(100).Column(x =>
                                {
                                    x.Item().Text($"Приём: {appointment.Id}").SemiBold();
                                    x.Item().Text($"Дата: {appointment.Date:yyyy-MM-dd HH:mm}");
                                });

                                row.RelativeItem().Column(x =>
                                {
                                    x.Item().Text($"Пациент: {appointment.PatientId}");
                                    x.Item().Text($"Врач: {appointment.DoctorId}");
                                });

                                row.RelativeItem().Column(x =>
                                {
                                    x.Item().Text($"Услуга: {appointment.ServiceId}");
                                    x.Item().Text($"Статус: {(appointment.IsApproved ? "Подтверждён" : "Ожидает")}");
                                });
                            });
                    });

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text("Жалобы:").SemiBold();
                        col.Item().Text(result.Complaints ?? "-");

                        col.Item().Text("Заключение:").SemiBold();
                        col.Item().Text(result.Conclusion ?? "-");

                        col.Item().Text("Рекомендации:").SemiBold();
                        col.Item().Text(result.Recommendations ?? "-");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(txt =>
                    {
                        txt.Span("Сгенерировано: ");
                        txt.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
                    });
            });
        });

        return document.GeneratePdf();
    }
}
