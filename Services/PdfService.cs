using PassportBookingReport.Reports;

namespace PassportBookingReport.Services;

public class PdfService
{
    public byte[] GenerateAppointmentPdf()
    {
        var report = new PassportAppointmentPdf();
        return report.Generate();
    }
}
