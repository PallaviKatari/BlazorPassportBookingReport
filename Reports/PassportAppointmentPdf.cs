using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using System.Text;

namespace PassportBookingReport.Reports;

public class PassportAppointmentPdf
{
    public byte[] Generate()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // ===== HEADER WITH LOGOS =====
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Image("wwwroot/images/logo-left.png");
                        row.RelativeItem().AlignCenter().Column(c =>
                        {
                            c.Item().Text("بسم الله الرحمن الرحيم").Bold();
                            c.Item().Text("وزارة الداخلية");
                            c.Item().Text("رئاسة قوات الشرطة");
                            c.Item().Text("الإدارة العامة للجوازات والهجرة");
                        });
                        row.RelativeItem().AlignRight().Image("wwwroot/images/logo-right.png");
                    });

                    col.Item().LineHorizontal(1);

                    // ===== BOOKING CODE + QR =====
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("كود الحجز: 1483452115").Bold();
                            c.Item().Text("تاريخ إنشاء الحجز: 07-02-2026");
                            c.Item().Text("رقم الهاتف: 249129005445");
                        });

                        row.ConstantItem(120).Height(120).Image(GenerateQr("https://passports.gov.sd/appointment/check/validity/1483452115"));
                    });

                    col.Item().AlignCenter().Text("مكتب جوازات سفارة جمهورية السودان بالرياض")
                        .FontSize(16).Bold();

                    col.Item().AlignCenter().Text("2026/02/11")
                        .FontSize(14).Bold();

                    col.Item().AlignCenter().Text("من الساعة 8 صباحا حتى الساعة 3 مساء");

                    col.Item().LineHorizontal(1);

                    // ===== PERSON TABLE =====
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("الاسم").Bold();
                            header.Cell().Text("الرقم الوطني").Bold();
                            header.Cell().Text("الجنس").Bold();
                            header.Cell().Text("صلة القرابة").Bold();
                            header.Cell().Text("تاريخ الميلاد/العمر").Bold();
                        });

                        table.Cell().Text("شيماء حسب الرسول علي الفكي");
                        table.Cell().Text("12047018785");
                        table.Cell().Text("أنثى");
                        table.Cell().Text("منشئ الطلب");
                        table.Cell().Text("1999-10-02 / 26 سنة");
                    });

                    col.Item().LineHorizontal(1);

                    // ===== REQUIREMENTS + NOTES =====
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("** المتطلبات **").Bold();
                            c.Item().Text("[1] استخراج جواز جديد");
                            c.Item().Text("- صورة مطبوعة من الرقم الوطني");
                            c.Item().Text("[2] تجديد جواز قديم");
                            c.Item().Text("- احضار اصل الجواز القديم");
                        });

                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("** تنبيهات **").Bold();
                            c.Item().Text("- تأكد من استيفاء المطلوبات جيداً");
                            c.Item().Text("- احضر في اليوم المحدد فقط");
                            c.Item().Text("- لا تتعامل مع وسطاء");
                        });
                    });

                    col.Item().LineHorizontal(1);

                    col.Item().AlignCenter().Text("يمكن التحقق من صحة هذا الحجز عبر الرابط التالي:");
                    col.Item().AlignCenter().Text("https://passports.gov.sd/appointment/check/validity/1483452115");
                });
            });
        }).GeneratePdf();
    }

    private byte[] GenerateQr(string url)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }
}
