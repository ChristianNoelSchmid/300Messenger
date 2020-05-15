using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.Images.ViewModels
{
    public class MessageImageViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public IFormFile FormFile { get; set; }
    }
}