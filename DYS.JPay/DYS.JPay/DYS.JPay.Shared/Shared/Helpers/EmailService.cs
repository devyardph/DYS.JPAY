using DYS.JPay.Shared.Shared.Dtos;
using MailKit.Net.Smtp;
using MimeKit;
using Org.BouncyCastle.Utilities;
using static System.Net.Mime.MediaTypeNames;
using Multipart = MimeKit.Multipart;


//vgaq qvpd nlcr rpch
namespace DYS.JPay.Shared.Shared.Helpers
{
    public interface IEmailService
    {
        Task<ResponseDto> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
    public class EmailService
    {
       public static async Task<ResponseDto> SendEmailAsync(
       string to,
       string subject,
       string body,
       string sender,
       string appPassword,
       byte[]? attachment = null,
       CancellationToken cancellationToken = default)
        {
            var output = new ResponseDto();
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("JPay", sender));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                // HTML body
                var bodyPart = new TextPart("html")
                {
                    Text = body
                };

                if (attachment != null)
                {
                    // Create a readable MemoryStream
                    var stream = new MemoryStream(attachment);
                    stream.Position = 0; // reset pointer
                    var file = new MimePart("text", "csv")
                    {
                        Content = new MimeContent(stream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = "sales_report.csv"
                    };

                    // Combine body + attachment
                    var multipart = new Multipart("mixed");
                    multipart.Add(bodyPart);
                    multipart.Add(file);
                    message.Body = multipart;
                }
                else
                {
                    message.Body = new TextPart("html")
                    {
                        Text = body
                    };
                }

                using (var client = new SmtpClient())
                {
                    // Use port 587 for TLS or 465 for SSL
                    await client.ConnectAsync("smtp.gmail.com", 587, false, cancellationToken);
                    await client.AuthenticateAsync(sender, appPassword, cancellationToken);
                    await client.SendAsync(message, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);
                }
                output.Success = true;
                output.Message = "Email sent successfully";
            }
            catch (Exception ex)
            {
                output.Success = false;
                output.Message = $"Email sending failed: {ex.Message}";
            }
            return output;
        }

    }
}

