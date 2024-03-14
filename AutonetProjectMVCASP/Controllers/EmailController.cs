using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Tasks;
using AutonetProjectMVCASP.Models;
using Microsoft.Extensions.Configuration;
using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using System.Configuration;
using System.Net;
using MailKit.Security;

namespace AutonetProjectMVCASP.Controllers
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAddress { get; set; }
        public bool UseSSL { get; set; }
    }
    public class EmailController : Controller
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailController(IConfiguration configuration)
        {
            _smtpSettings = new SmtpSettings();
            configuration.GetSection("SmtpSettings").Bind(_smtpSettings);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("carservicemanagerproject@gmail.com", "carservicemanagerproject@gmail.com"));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.Auto); // Connect to the SMTP server
                await client.AuthenticateAsync("carservicemanagerproject@gmail.com", "krwcxbwjwoozckvu"); // Authenticate if required
                await client.SendAsync(message); // Send the message
                await client.DisconnectAsync(true); // Disconnect from the SMTP server
            }
        }
    }
}
