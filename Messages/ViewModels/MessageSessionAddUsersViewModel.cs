using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Messages.ViewModels
{
    public class MessageSessionAddUsersViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        [Required]
        public int SessionId { get; set; }

        public string[] Values { get; set; }
    } 
}