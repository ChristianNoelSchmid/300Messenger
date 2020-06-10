using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class AuthorizedJwtViewModel
    {
        [Required]
        public string JwtFrom { get; set; }
    }
}