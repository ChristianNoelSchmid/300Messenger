using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class AuthorizedQueryViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        public string Value { get; set; }
    }
}