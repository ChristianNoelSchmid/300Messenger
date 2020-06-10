using System.ComponentModel.DataAnnotations;

namespace Messages.ViewModels
{
    public class MessageSessionAddRemoveUsersViewModel
    {
        [Required]
        public string FromJwt { get; set; }

        [Required]
        public int SessionId { get; set; }

        [Required]
        public string Secret { get; set; }

        public string[] Values { get; set; }
    } 
}