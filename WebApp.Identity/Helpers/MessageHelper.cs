using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace WebApp.Identity.Helpers
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public int DeliveryMethod { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DescriptPass
        { get
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(Password));
            }
        }
    }

    public static class MessageHelper
    {
        public static bool SendMail(SmtpSettings settings)
        {
            var to = new MailAddress("lamattina@live.com", "Eduardo Lamattina");
            var from = new MailAddress(settings.Email, settings.Name);
            string host = settings.Host;
            var subject = "Using new SMTP Cliente";
            var body = "Using new feature";

            var smtp = new SmtpClient
            {
                Host = settings.Host,
                Port = settings.Port,
                EnableSsl = settings.SSL,
                DeliveryMethod = (SmtpDeliveryMethod)settings.DeliveryMethod,
                UseDefaultCredentials = settings.UseDefaultCredentials,
                Credentials = new NetworkCredential(settings.Email, settings.DescriptPass)
            };

            using (var message = new MailMessage(from, to) { Subject = subject, Body = body })
            {
                try
                {
                    smtp.Send(message);

                    smtp.SendCompleted += (s, e) =>
                    {
                        smtp.Dispose();
                    };

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
