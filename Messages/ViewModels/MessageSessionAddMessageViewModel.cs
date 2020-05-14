using System.ComponentModel.DataAnnotations;
using _300Messenger.Messages.Models;

namespace _300Messenger.Messages.ViewModels
{
    public class MessageSessionAddMessageViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        [Required]
        public int SessionId { get; set; }

        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}