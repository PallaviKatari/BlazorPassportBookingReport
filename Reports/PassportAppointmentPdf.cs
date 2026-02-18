using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
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

    //https://passports.gov.sd/appointment/check/validity/1483452115  -- for your internet - testing - public use only
    // Need to check whether the QR code is accesible in a LAN environment without internet access, as the URL is public and may not be accessible in all environments.
    // If the QR code needs to work in a LAN environment, consider using a local URL or IP address that can be accessed within the network.
    private byte[] GenerateQr(string url)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }
    //csharp Reports/PassportAppointmentPdf.cs
//private byte[] GenerateQrForAppointment(string appointmentId, bool preferLan = false, string lanHost = null, string hmacSecret = null)
//    {
//        // Build content:
//        // - preferLan + lanHost -> "http://{lanHost}/appointment/check/validity/{id}"
//        // - otherwise -> public URL
//        // - if lanHost is null and preferLan is true, embed a signed payload instead.
//        string content;

//        if (preferLan && !string.IsNullOrWhiteSpace(lanHost))
//        {
//            // Make sure lanHost is reachable by clients (IP or hostname resolvable via local DNS/mDNS)
//            content = $"http://{lanHost}/appointment/check/validity/{appointmentId}";
//        }
//        else if (!preferLan)
//        {
//            content = $"https://passports.gov.sd/appointment/check/validity/{appointmentId}";
//        }
//        else
//        {
//            // Fallback: embed data so it can be validated offline.
//            // Optionally sign the payload with a server-side secret to prevent tampering.
//            var payload = new StringBuilder();
//            payload.Append($"id:{appointmentId};ts:{DateTime.UtcNow:yyyy-MM-ddTHH:mmZ}");
//            if (!string.IsNullOrEmpty(hmacSecret))
//            {
//                using var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(hmacSecret));
//                var sig = Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload.ToString())));
//                payload.Append($";sig:{sig}");
//            }
//            content = payload.ToString();
//        }

//        // Reuse existing QR generation code
//        using var qrGenerator = new QRCoder.QRCodeGenerator();
//        var qrData = qrGenerator.CreateQrCode(content, QRCoder.QRCodeGenerator.ECCLevel.Q);
//        var qrCode = new QRCoder.PngByteQRCode(qrData);
//        return qrCode.GetGraphic(20);
//    }
}
