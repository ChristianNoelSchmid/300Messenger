using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.ViewModels
{
    public class AuthorizedProfileImageViewModel
    { 
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public bool IsThumb { get; set; }
    }
}
