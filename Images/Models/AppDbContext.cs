using Microsoft.EntityFrameworkCore;

namespace _300Messenger.Images.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        DbSet<ProfilePhotoPath> ProfilePhotoPaths { get; set; }
    }
}