using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Friendships.ViewModels
{
    public class FriendshipViewModel
    {
        [Required]
        public string FromJwt { get; set; }
        [Required]
        public string OtherEmail { get; set; }

        public override string ToString()
        {
            return $"{{ To: {OtherEmail}, JWT: {FromJwt} }}";
        }
    }
}