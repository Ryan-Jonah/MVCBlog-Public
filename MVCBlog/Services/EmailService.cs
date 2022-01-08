using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MVCBlog.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCBlog.Services
{
    public class EmailService : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        //Retreive configured mail settings from Startup.cs(Services)
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        //This is the content of the SendContactEmailAsync() method
        //The functional portion of this method is the same as SendEmailAsync(), but the body is pre-set
        public async Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(_mailSettings.Mail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = $"<b>{name}</b> has sent you an email and can be reached at: <b>{emailFrom}</b><br/><br/>{htmlMessage}";

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
        }

        public async Task SendEmailAsync(string emailTo, string subject, string htmlMessage)
        {
            //Internet Standard - Multipurpose Internet Mail Extensions(MIME)
            var email = new MimeMessage();

            //Gathers sender email from appsettings.json
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);

            //Gather receiver email as method parameter
            email.To.Add(MailboxAddress.Parse(emailTo));

            //Subject of the email
            email.Subject = subject;

            //Builds the body of the email message
            var builder = new BodyBuilder()
            {
                HtmlBody = htmlMessage
            };

            //Assigning the built message
            email.Body = builder.ToMessageBody();

            //Simple Mail Transfer Protocol client creation
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
