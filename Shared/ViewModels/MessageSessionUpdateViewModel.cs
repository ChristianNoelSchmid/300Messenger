using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.ViewModels
{
    public class MessageSessionUpdateViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string JwtFrom { get; set; }
        
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public List<string> Emails { get; set; }
    }
}