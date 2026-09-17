namespace TopSpeed.Web.Services
{
    using Microsoft.AspNetCore.Identity.UI.Services;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("ketheesabi7400@gmail.com", "talx liby jntt tplt"),
                EnableSsl = true,
            };

            using var mailMessage = new MailMessage("ketheesabi7400@gmail.com", email, subject, message)
            {
                IsBodyHtml = true
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
