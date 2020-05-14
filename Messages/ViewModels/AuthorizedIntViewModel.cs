using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Messages.ViewModels
{
    public class AuthorizedIntViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        public int Value { get; set; }
    }
}