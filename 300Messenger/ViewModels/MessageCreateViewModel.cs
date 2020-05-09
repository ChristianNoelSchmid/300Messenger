using System.ComponentModel.DataAnnotations;
using _300Messenger.Models;

namespace _300Messenger.ViewModels
{
    public class MessageCreateViewModel
    {
        public int MessageSessionId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}