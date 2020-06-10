using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class MessageSessionCreateViewModel
    {
        [Required]
        public string FromJwt { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Emails { get; set; }
    }
}