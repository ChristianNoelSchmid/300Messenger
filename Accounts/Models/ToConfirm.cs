using System;
using System.ComponentModel.DataAnnotations;

namespace Accounts.Models
{
    public class ToConfirm
    {
        public int Id { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public int Token { get; set; }
        [Required]
        public string EmailToConfirm { get; set; }
    }
}