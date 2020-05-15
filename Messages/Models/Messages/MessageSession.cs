using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.Messages.Models
{
    /// <summary>
    /// Stores all information pertaining to a single Message Session
    /// Includes email references to all Users included, and
    /// a List of Messages to represent all Messages in the Session
    /// </summary>
    public class MessageSession
    {
        public int Id { get; set; }

        /// <summary>
        /// The main title of the MessageSession
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A description of the MessageSession
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The email addresses of the Users included in
        /// the MessageSession. Seperated by ';'
        /// The first Email represents the User who posted the
        /// MessageSession.
        /// </summary>
        [Required(ErrorMessage="Please enter your email address")]
        [EmailAddress(ErrorMessage="Please enter a valid email address")]
        public string Emails { get; set; }
    }
}
