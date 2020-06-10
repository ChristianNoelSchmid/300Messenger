using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.ViewModels
{
    public class AuthorizedEmailViewModel
    {
        [Required]
        public string JwtFrom { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
