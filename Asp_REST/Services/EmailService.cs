using System.Net;
using System.Net.Mail;
using System.Text;

namespace Asp_REST.Services
{
    public class EmailService(IConfiguration configuration)
    {
        
           
        public async Task SendEmail(string to, string subject, string text)
        {
            var username = configuration.GetValue<string>("EmailConfig:Username");
            var password = configuration.GetValue<string>("EmailConfig:Password");
            var host = configuration.GetValue<string>("EmailConfig:Host");
            var port = configuration.GetValue<int>("EmailConfig:Port");

            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(username, password);

            var message = new MailMessage(username, to, subject, text)
            {
                IsBodyHtml = true,  
                BodyEncoding = Encoding.UTF8,  
                SubjectEncoding = Encoding.UTF8 
            };
            await smtpClient.SendMailAsync(message);

        }
    }
}
