using System.Threading.Tasks;

namespace Images.Models
{
    public interface IProfilePhotoPathRepo
    {
        Task<ProfilePhotoPath> AddPhotoPath(ProfilePhotoPath photoPath); 
        Task<ProfilePhotoPath> GetPhotoPath(string email); 
        Task<ProfilePhotoPath> RemovePhotoPath(string email);
        Task<ProfilePhotoPath> UpdatePhotoPath(string email, string photoPath);
    }
}