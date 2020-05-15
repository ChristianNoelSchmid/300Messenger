using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Messages.ViewModels
{
    public class MessageSessionCreateViewModel
    {
        [Required]
        public string FromJwt { get; set; }
        
        [Required]
        public string Secret { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public List<string> Emails { get; set; }
    }
}