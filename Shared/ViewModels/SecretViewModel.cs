using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.ViewModels
{
    public class SecretViewModel
    {
        [Required]
        public string Secret { get; set; }
    }
}
