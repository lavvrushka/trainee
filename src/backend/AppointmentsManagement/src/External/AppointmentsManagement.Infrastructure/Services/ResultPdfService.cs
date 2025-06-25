using AppointmentsManagement.Application.Common.Interfaces.IServices;
using AppointmentsManagement.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AppointmentsManagement.Infrastructure.Services;

public class ResultPdfService : IResultPdfService
{
    public byte[] GenerateResultPdf(ResultDto result)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text($"Результат приёма №{result.Id}")
                    .SemiBold().FontSize(16).FontColor(Colors.Blue.Medium);

                page.Content()
                    .Column(col =>
                    {
                        col.Spacing(5);

                        col.Item().Text($"AppointmentId: {result.AppointmentId}");
                        col.Item().Text($"Complaints: {result.Complaints}");
                        col.Item().Text($"Conclusion: {result.Conclusion}");
                        col.Item().Text($"Recommendations: {result.Recommendations}");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(txt =>
                    {
                        txt.Span("Generated on ");
                        txt.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"));
                    });
            });
        });

        return document.GeneratePdf();
    }
}
