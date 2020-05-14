using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Accounts.ViewModels
{
    public class AuthorizedQueryViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        public string Value { get; set; }
    }
}