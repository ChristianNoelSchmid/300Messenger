using System;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public enum MessageType
    {
        Text,
        Image
    }

    public class Message
    {
        public int Id { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// The Email of the User whom posted the Message
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// The Type of Message represented
        /// </summary>
        public MessageType Type { get; set; }

        /// <summary>
        /// The message content. If Type is Text, Content
        /// will be a directory to the image in the database. 
        /// If Type is Text, Content will be the actual text.
        /// </summary>
        [Required]
        public string Content { get; set; }

         /*
         * MessageSession to Message Table Relationship
         * Properties
         */
        /// <summary>
        /// The Messages associated with the MessageSession.
        /// A One-To-Many Relationship.
        /// TODO -- Offload images onto seperate server for better
        ///      application security
        public int MessageSessionId { get; set; }
    }
}