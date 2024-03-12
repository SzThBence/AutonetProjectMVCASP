using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using AutonetProjectMVCASP.Models;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

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

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            using (var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
                client.EnableSsl = _smtpSettings.UseSSL;

                var message = new MailMessage(_smtpSettings.FromAddress, toEmail)
                {
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(message);
            }
        }
    }
}
