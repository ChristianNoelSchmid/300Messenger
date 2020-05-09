using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.Models
{
    /// <summary>
    /// Stores all information pertaining to a single Message
    /// Includes reference to the User who posted it
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
        /// The email address of the poster
        /// TODO -- instead of an Email address, create a
        ///      reference to a User in the database
        /// </summary>
        [Required(ErrorMessage="Please enter your email address")]
        [EmailAddress(ErrorMessage="Please enter a valid email address")]
        public string Email { get; set; }

        /*
         * MessageSession to Message Table Relationship
         * Properties
         */
        /// <summary>
        /// The Messages associated with the MessageSession.
        /// A One-To-Many Relationship.
        /// TODO -- Offload images onto seperate server for better
        ///      application security
        public List<Message> Messages { get; set; }
    }
}
