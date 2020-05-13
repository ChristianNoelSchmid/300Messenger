using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Shared.ViewModels
{
    public class FriendshipViewModel
    {
        [Required]
        [StringLength(240)]
        public string FromJwtToken { get; set; }
        [Required]
        public string ToEmail { get; set; }
    }
}