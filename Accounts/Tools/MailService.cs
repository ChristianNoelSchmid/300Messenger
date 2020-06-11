using Accounts.Models;
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
    /// <summary>
    /// Handles sending emails to a client User's email address
    /// Used to handle sending confirmation emails when a User
    /// registers.
    /// </summary>
    public class MailService : IMailService
    {
        private readonly string _filename = "ConfirmAddress.html";
        private readonly MailAddress FromAddress;
        private readonly string FromPassword;
        private readonly SmtpClient _client;
        
        public MailService()
        {
            FromAddress = new MailAddress(
                Environment.GetEnvironmentVariable("CONFIRMATIONEMAIL_SENDERADDRESS"),
                "300Messenger"
            );
            FromPassword =
                Environment.GetEnvironmentVariable("CONFIRMATIONEMAIL_SENDERPASSWORD");

            // Create the client, using supplied information in Properties/launchSettings.json
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

        /// <summary>
        /// Sends an email, using a ToConfirm token to represent
        /// the link value.
        /// </summary>
        public void SendConfirmationEmail(ToConfirm toConfirm)
        {
            var staticIp = Environment.GetEnvironmentVariable("STATICIP");
            var body = File.ReadAllText(_filename)
                .Replace("{{LINK}}", $"https://{staticIp}:5005/Account/ConfirmEmail?token={toConfirm.Token}");

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