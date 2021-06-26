using Mailjet.Client;
using Mailjet.Client.Resources;
//using MailKit.Net.Smtp;
//using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using SendGrid;
//using SendGrid.Helpers.Mail;
//using MailKit.Security;

namespace Rocky_Utility
{
    //public class EmailSender : IEmailSender // this line is written by me
    //{
        //    public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //    {
        //        return Execute(email, subject, htmlMessage);
        //    }
        //    public async Task Execute(string email, string subject, string body)
        //    {
        //        var emailMessage = new MimeMessage();

        //        emailMessage.From.Add(new MailboxAddress("Smit Vora", "smitvora10@gmail.com"));
        //        emailMessage.To.Add(new MailboxAddress("", email));
        //        emailMessage.Subject = subject;
        //        emailMessage.Body = new TextPart("Email is Confirmed") { Text = body };

        //        using (var client = new SmtpClient())
        //        {
        //            client.LocalDomain = "www.darshan.ac.in";
        //            await client.ConnectAsync("smtp.relay.uri", 25, SecureSocketOptions.None).ConfigureAwait(false);
        //            await client.SendAsync(emailMessage).ConfigureAwait(false);
        //            await client.DisconnectAsync(true).ConfigureAwait(false);
        //        }
        //    }
        //}
        public class EmailSender : IEmailSender
        {
            public Task SendEmailAsync(string email, string subject, string htmlMessage)
            {
                return Execute(email, subject, htmlMessage);
            }


            public async Task Execute(string email, string subject, string body)
            {

                MailjetClient client = new MailjetClient("ea9a3c8eeae3a6b2d7b10b2d994178c6", "37b8deb07281dca12981e1cd233d092b")
                {
                    //Version = ApiVersion.V3_1,

                };
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                 .Property(Send.Messages, new JArray {
                    new JObject {
                     {
                      "From",
                      new JObject {
                       {"Email", "smitv803@gmail.com"},
                       {"Name", "Smit"}
                      }
                     }, {
                      "To",
                      new JArray {
                       new JObject {
                        {
                         "Email",
                         email
                        }, {
                         "Name",
                         "Rocky"
                        }
                       }
                      }
                     }, {
                      "Subject",
                      subject
                     },
                        {
                      "HTMLPart",
                      body
                           },

                    }
                 });
                await client.PostAsync(request);

            }
        }
}
