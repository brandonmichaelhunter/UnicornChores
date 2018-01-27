using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ParentsRules.Services;
using System.IO;

namespace ParentsRules.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string Recipent, string email, string link, string WebsiteUrl)
        {
           

            string message = EmailConformationBody(Recipent,HtmlEncoder.Default.Encode(link), HtmlEncoder.Default.Encode(WebsiteUrl));// $"Hey {ParentFirstName}, {AuthorName} has invited you to join his account on Unicorn Chores.</br>Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a> to create your new account.";
            return emailSender.SendEmailAsync(email, "Confirm your email", message);
        }

        public static Task SendParentRequestEmailConfirmationAsync(this IEmailSender emailSender, string email, string link, string AuthorName, string ParentFirstName, string WebsiteUrl)
        {
            string subject = string.Format("{0} has sent you a request to join Unicorn Chores", AuthorName);
            string message = ParentRequestBody(AuthorName, ParentFirstName, HtmlEncoder.Default.Encode(link), HtmlEncoder.Default.Encode(WebsiteUrl));
            return emailSender.SendEmailAsync(email, subject, message);
             
        }
        //public static Task SendFriendRequestEmailConfirmationAsync(this IEmailSender emailSender, string email, string link, string AuthorName, string ParentFirstName)
        //{
        //    string subject = string.Format("{0} has sent you a request to join Unicorn Chores", AuthorName);
        //    string message = SendFriendRequestEmailTemplate(AuthorName, ParentFirstName, link);// $"Hey {ParentFirstName}, {AuthorName} has invited you to join his account on Unicorn Chores.</br>Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a> to create your new account.";
        //    return emailSender.SendEmailAsync(email, subject, message);
        //    //$"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        //}

        private static string ParentRequestBody(string Requestor, string Requeste, string RegistraionLink, string WebsiteUrl)
        {
           
            string emailBody = string.Empty;
            //using streamreader for reading my htmltemplate   

            using (StreamReader reader = new StreamReader("./Assets/SendFriendRequestEmail.html"))

            {

                emailBody = reader.ReadToEnd();

            }

            emailBody = emailBody.Replace("{Requestor}", Requestor); //replacing the required things  

            emailBody = emailBody.Replace("{RegistrationLink}", RegistraionLink);

            emailBody = emailBody.Replace("{Requeste}", Requeste);
            emailBody = emailBody.Replace("{WebsiteUrl}", WebsiteUrl);

            return emailBody;

        }
        private static string EmailConformationBody(string Requeste, string ConformationLink, string WebsiteUrl)
        {
            string emailBody = string.Empty;
            //using streamreader for reading my htmltemplate   

            using (StreamReader reader = new StreamReader("./Assets/EmailConformation.html"))

            {

                emailBody = reader.ReadToEnd();

            }



            emailBody = emailBody.Replace("{ConformationLink}", ConformationLink);

            emailBody = emailBody.Replace("{Requeste}", Requeste);
            emailBody = emailBody.Replace("{WebsiteUrl}", WebsiteUrl);

            return emailBody;
        }
    }
}
