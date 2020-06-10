using Accounts.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Tools
{
    public class MailService : IMailService
    {
        private readonly string _filename = "ConfirmAddress.html";
        private readonly MailAddress FromAddress;
        private readonly string FromPassword;
        private readonly SmtpClient _client;

        private static string[] Scopes = { GmailService.Scope.GmailReadonly };
        private static string ApplicationName = "300Messenger - Accounts Gmail API";

        private static UserCredential _credential;

        private static void AuthenticateGmailServices()
        {
 
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

            // List labels.
            IList<Label> labels = request.Execute().Labels;
            Console.WriteLine("Labels:");
            if (labels != null && labels.Count > 0)
            {
                foreach (var labelItem in labels)
                {
                    Console.WriteLine("{0}", labelItem.Name);
                }
            }
            else
            {
                Console.WriteLine("No labels found.");
            }
            Console.Read();

        }

        public MailService()
        {
            AuthenticateGmailServices();

            FromAddress = new MailAddress(
                Environment.GetEnvironmentVariable("CONFIRMATIONEMAIL_SENDERADDRESS"),
                "300Messenger"
            );
            FromPassword =
                Environment.GetEnvironmentVariable("CONFIRMATIONEMAIL_SENDERPASSWORD");

            _client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(FromAddress.Address, FromPassword)
            };
        }

        public void SendConfirmationEmail(ToConfirm toConfirm)
        {
            var body = File.ReadAllText(_filename)
                .Replace("{{LINK}}", $"https://localhost:5005/Account/ConfirmEmail?token={toConfirm.Token}");

            var toAddress = new MailAddress(toConfirm.EmailToConfirm, "To Name");
            using (var message = new MailMessage(FromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = "300Messenger - please confirm your email",
                Body = body
            })
            {
                _client.Send(message);
            }
        }
    }
}