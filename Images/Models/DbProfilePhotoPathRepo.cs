using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Images.Models
{
    public class DbProfilePhotoPathRepo : IProfilePhotoPathRepo
    {
        private readonly AppDbContext dbContext;

        public DbProfilePhotoPathRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ProfilePhotoPath> AddPhotoPath(ProfilePhotoPath photoPath)
        {
            await dbContext.ProfilePhotoPaths.AddAsync(
                photoPath 
            );
            await dbContext.SaveChangesAsync();

            return photoPath;
        }

        public async Task<ProfilePhotoPath> GetPhotoPath(string email)
        {
            return await dbContext.ProfilePhotoPaths.FirstOrDefaultAsync(
                p => p.Email == email
            );
        }

        public async Task<ProfilePhotoPath> RemovePhotoPath(string email)
        {
            var photoPath = await dbContext.ProfilePhotoPaths.FirstOrDefaultAsync(
                p => p.Email == email
            );

            if(photoPath != null)
            {
                dbContext.ProfilePhotoPaths.Remove(photoPath);
                await dbContext.SaveChangesAsync();
            }

            return photoPath;
        }

        public async Task<ProfilePhotoPath> UpdatePhotoPath(string email, string photoPath)
        {
            var profilePhotoPath = await dbContext.ProfilePhotoPaths.FirstOrDefaultAsync(
                p => p.Email == email
            );

            if(profilePhotoPath != null)
            {
                profilePhotoPath.PhotoPath = photoPath;
                await dbContext.SaveChangesAsync();
            }

            return profilePhotoPath;
        }
    }
}