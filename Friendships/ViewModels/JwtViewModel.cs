using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Friendships.ViewModels
{
    public class JwtViewModel
    {
        [Required]
        public string FromJwt { get; set; }
    }
}