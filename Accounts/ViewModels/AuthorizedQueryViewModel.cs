using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Authentication.ViewModels
{
    public class AuthorizedQueryViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        public string Value { get; set; }
    }
}