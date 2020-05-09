using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Shared
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}