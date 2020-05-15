using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Messages.ViewModels
{
    public class AuthorizedJwtViewModel
    {
        [Required]
        public string FromJwt { get; set; }
    }
}