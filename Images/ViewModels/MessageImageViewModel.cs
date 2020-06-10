using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Images.ViewModels
{
    public class MessageImageViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public IFormFile FormFile { get; set; }

        [Required]
        public string Secret { get; set; }
    }
}