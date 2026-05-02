using MimeKit;
using MailKit.Net.Smtp;
using DYS.JPay.Shared.Shared.Dtos;

//vgaq qvpd nlcr rpch
namespace DYS.JPay.Shared.Shared.Helpers
{
    public interface IEmailService
    {
        Task<ResponseDto> SendEmailAsync(string to, string subject, string body);
    }
    public class EmailService
    {
       public static async Task<ResponseDto> SendEmailAsync(
       string to,
       string subject,
       string body,
       string sender,
       string appPassword)
        {
            var output = new ResponseDto();
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("JPay", sender));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    // Use port 587 for TLS or 465 for SSL
                    await client.ConnectAsync("smtp.gmail.com", 587, false);
                    await client.AuthenticateAsync(sender, appPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
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

