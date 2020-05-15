using System.ComponentModel.DataAnnotations;

namespace _300Messenger.Images.Models
{
    public class ProfilePhotoPath
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhotoPath { get; set; }
    }
}