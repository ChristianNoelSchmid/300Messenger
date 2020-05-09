using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using _300Messenger.Tools;
using _300Messenger.ValidationAttributes;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.ViewModels
{
    /// <summary>
    /// ModelView that handles creation of a new Message
    /// </summary>
    public class MessageSessionCreateViewModel
    {
        /// <summary>
        /// The main title of the MessageSession
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A description of the MessageSession
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The email address of the poster
        /// TODO -- instead of an Email address, create a
        ///      reference to a User in the database
        /// </summary>
        [Required(ErrorMessage="Please enter your email address")]
        [EmailAddress(ErrorMessage="Please enter a valid email address")]
        public string Email { get; set; }
    }
}