using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Friendships.ViewModels
{
    public class JWTViewModel
    {
        [Required]
        public string FromJwt { get; set; }
    }
}