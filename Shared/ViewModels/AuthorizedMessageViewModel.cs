using Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class AuthorizedMessageViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        [Required]
        public int SessionId { get; set; }

        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}