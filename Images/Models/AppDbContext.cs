using Microsoft.EntityFrameworkCore;

namespace Images.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<ProfilePhotoPath> ProfilePhotoPaths { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProfilePhotoPath>().HasData(
                new ProfilePhotoPath { Id=1, Email="pete@fakemail.com", PhotoPath="Media/Profiles/pete.png" },
                new ProfilePhotoPath { Id=2, Email="roger@fakemail.com", PhotoPath="Media/Profiles/roger.png" },
                new ProfilePhotoPath { Id=3, Email="john@fakemail.com", PhotoPath="Media/Profiles/john.png" },
                new ProfilePhotoPath { Id=4, Email="keith@fakemail.com", PhotoPath="Media/Profiles/keith.png" },
                new ProfilePhotoPath { Id=9, Email="jimmi@fakemail.com", PhotoPath="Media/Profiles/jimmi.png" },
                new ProfilePhotoPath { Id=5, Email="geddy@fakemail.com", PhotoPath="Media/Profiles/geddy.png" },
                new ProfilePhotoPath { Id=6, Email="alex@fakemail.com", PhotoPath="Media/Profiles/alex.png" },
                new ProfilePhotoPath { Id=7, Email="neil@fakemail.com", PhotoPath="Media/Profiles/neil.png" },
                new ProfilePhotoPath { Id=8, Email="freddie@fakemail.com", PhotoPath="Media/Profiles/freddie.png" }
            );
        }
    }
}