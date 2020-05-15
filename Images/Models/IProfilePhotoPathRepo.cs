using System.Threading.Tasks;

namespace _300Messenger.Images.Models
{
    public interface IProfilePhotoPathRepo
    {
        Task<ProfilePhotoPath> AddPhotoPath(ProfilePhotoPath photoPath); 
        Task<string> GetPhotoPath(string email); 
        Task<ProfilePhotoPath> RemovePhotoPath(string email);
        Task<ProfilePhotoPath> UpdatePhotoPath(string email, string photoPath);
    }
}