using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace _300Messenger.Images.ViewModels
{
    public class ProfileImageViewModel
    {
        [Required]
        public string FromJwt { get; set; }
        [Required]
        public IFormFile FormFile { get; set; }

    }
}