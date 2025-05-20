using Microsoft.Extensions.Configuration;
using quangcao.Models.ViewModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;

public class GmailEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public GmailEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        try
        {
            // Lấy thông tin cấu hình từ appsettings.json
            var smtpSettings = _config.GetSection("GoogleKey").GetSection("SMTPSettings");

            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"];
            var fromName = smtpSettings["FromName"];

            using (var smtpClient = new SmtpClient(host))
            {
                smtpClient.Port = port;
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("✅ Email đã được gửi thành công!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Đã xảy ra lỗi khi gửi email: {ex.Message}");
        }
    }
}
