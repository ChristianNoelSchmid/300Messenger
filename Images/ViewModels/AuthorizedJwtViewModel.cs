using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Images.ViewModels
{
    public class AuthorizedJwtViewModel
    {
        [Required]
        public string FromJwt { get; set; }
    }
}