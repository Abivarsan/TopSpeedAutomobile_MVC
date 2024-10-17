namespace TopSpeed.Web.Services
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public EmailSender()
        {
            _smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,  // Or the port required by your SMTP provider
                Credentials = new NetworkCredential("ketheesabi7400@gmail.com", "talx liby jntt tplt"),
                EnableSsl = true,
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var mailMessage = new MailMessage("ketheesabi7400@gmail.com", email, subject, message)
            {
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
