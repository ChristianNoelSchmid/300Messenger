using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class AuthorizedIntViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        [Required]
        public int Value { get; set; }
    }
}