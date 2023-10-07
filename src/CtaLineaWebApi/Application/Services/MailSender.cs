using CtaLineaWebApi.Application.Model;
using CtaLineaWebApi.Configuration;
using MimeKit;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Services
{
    // https://code-maze.com/aspnetcore-send-email/
    public class MailSender 
        : IMailSender
    {
        private readonly EmailConfiguration _emailConfig;

        public MailSender(
            EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
            if (string.IsNullOrWhiteSpace(_emailConfig.FromName)) _emailConfig.FromName = _emailConfig.From;
            if (string.IsNullOrWhiteSpace(_emailConfig.Reply)) _emailConfig.Reply = _emailConfig.From;
            if (string.IsNullOrWhiteSpace(_emailConfig.ReplyName)) _emailConfig.ReplyName = _emailConfig.Reply;
            
            if (string.IsNullOrWhiteSpace(_emailConfig.BccName)) _emailConfig.FromName = _emailConfig.Bcc;
        }

        public async Task SendEmailAsync(
            MailMessage message)
        {
            var emailMessage = this.CreateEmailMessage(message);
            await this.SendAsync(emailMessage)
                .ConfigureAwait(false);
        }

        private MimeMessage CreateEmailMessage(MailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.FromName, _emailConfig.From));
            emailMessage.ReplyTo.Add(new MailboxAddress(_emailConfig.ReplyName, _emailConfig.Reply));
            emailMessage.To.AddRange(message.To);
            if (string.IsNullOrWhiteSpace(_emailConfig.Bcc) == false)
            {
                emailMessage.Bcc.Add(new MailboxAddress(_emailConfig.BccName, _emailConfig.Bcc));
            }
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            return emailMessage;
        }

        private async Task SendAsync(
            MimeMessage mailMessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(
                        _emailConfig.SmtpServer,
                        _emailConfig.Port,
                        _emailConfig.SecureSocketOptions
                        // MailKit.Security.SecureSocketOptions.None
                        // MailKit.Security.SecureSocketOptions.StartTls
                        );

                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(mailMessage)
                        .ConfigureAwait(false);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
