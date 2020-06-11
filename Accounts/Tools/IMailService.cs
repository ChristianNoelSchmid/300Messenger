using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Tools
{
    /// <summary>
    /// Interface for MailingService
    /// For dependency injection
    /// </summary>
    public interface IMailService
    { 
        public void SendConfirmationEmail(ToConfirm toConfirm);
    }
}
