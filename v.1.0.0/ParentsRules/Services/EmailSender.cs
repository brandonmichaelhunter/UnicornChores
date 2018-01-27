using Microsoft.Extensions.Options;
using ParentsRules.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
namespace ParentsRules.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            Execute(email, subject, message).Wait();
            //return Task.CompletedTask;
            return Task.FromResult(0);
        }
        public EmailSettings _emailSettings { get; }
        public async Task Execute(string email, string subject, string message)
        {
            try
            {
                //string toEmail = string.IsNullOrEmpty(email)
                //                 ? _emailSettings.ToEmail
                //                 : email;
                //MailMessage mail = new MailMessage()
                //{
                //    From = new MailAddress("brandonmichaelhunter@gmail.com", "New User")
                //};
                //mail.To.Add(new MailAddress(toEmail));
                ////mail.CC.Add(new MailAddress(_emailSettings.CcEmail));

                //mail.Subject = "Unicorn Chores - " + subject;
                //mail.Body = message;
                //mail.IsBodyHtml = true;

                //mail.Priority = MailPriority.High;

                //using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                //{
                //    smtp.DeliveryFormat = SmtpDeliveryFormat.SevenBit;
                //    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                //    smtp.EnableSsl = true;
                //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    smtp.UseDefaultCredentials = false;
                //    await smtp.SendMailAsync(mail);
                //}
                // Create a SendGridMessage object.
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress(_emailSettings.UsernameEmail, "Unicorn Chores Team"));

                // Set the recipients
                var recipients = new List<EmailAddress> {
                    new EmailAddress(email)
                };
                msg.AddTos(recipients);
                msg.SetSubject("Unicorn Chores - " + subject);
                msg.AddContent(MimeType.Html, message);
                var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                var client = new SendGridClient(apiKey);
                var response = await client.SendEmailAsync(msg);
                //Log the response value.
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Error: {0}", ex.Message.ToString()));
            }
        }
    }
}
