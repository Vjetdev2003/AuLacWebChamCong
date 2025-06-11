using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
namespace AuLacChamCong.Services.Modules.Email
{
    public class SendMessage
    {
        private readonly IConfiguration _configuration;
        public SendMessage(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> SendEmailToAdmin(string toEmail, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["EmailSettings:SenderName"],
                    _configuration["EmailSettings:SenderEmail"]
                ));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body // Đã sửa lỗi thiếu biến body
                };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["EmailSettings:SMTPServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return "Email sent successfully"; // Trả về thông báo thành công
            }
            catch (Exception ex)
            {
                return $"Error sending email: {ex.Message}"; // Bắt lỗi và trả về thông báo lỗi
            }
        }
    }
}
