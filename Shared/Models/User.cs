using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set;}

        public bool EmailConfirmed { get; set; } = false;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public string PasswordHash { get; set; }
    }
}