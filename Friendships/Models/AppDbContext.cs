using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Friendships.Models
{
    /// <summary>
    /// The database context for the microservice
    /// Connects with the Friendships database
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Friendship> Friendships { get; set; }

        /// <summary>
        /// Seed Friendship data into the database upon creation
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Friendship>().HasData(
                new Friendship
                {
                    Id = 1,
                    RequesterEmail = "pete@fakemail.com",
                    ConfirmerEmail = "roger@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 2,
                    RequesterEmail = "pete@fakemail.com",
                    ConfirmerEmail = "john@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 3,
                    RequesterEmail = "pete@fakemail.com",
                    ConfirmerEmail = "keith@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 4,
                    RequesterEmail = "roger@fakemail.com",
                    ConfirmerEmail = "john@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 5,
                    RequesterEmail = "roger@fakemail.com",
                    ConfirmerEmail = "keith@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 6,
                    RequesterEmail = "keith@fakemail.com",
                    ConfirmerEmail = "john@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 7,
                    RequesterEmail = "geddy@fakemail.com",
                    ConfirmerEmail = "neil@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 8,
                    RequesterEmail = "geddy@fakemail.com",
                    ConfirmerEmail = "alex@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 9,
                    RequesterEmail = "alex@fakemail.com",
                    ConfirmerEmail = "neil@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 10,
                    RequesterEmail = "jimmi@fakemail.com",
                    ConfirmerEmail = "freddie@fakemail.com",
                    IsConfirmed = true 
                },
                new Friendship
                {
                    Id = 11,
                    RequesterEmail = "jimmi@fakemail.com",
                    ConfirmerEmail = "pete@fakemail.com",
                    IsConfirmed = true 
                },
                new Friendship
                {
                    Id = 12,
                    RequesterEmail = "jimmi@fakemail.com",
                    ConfirmerEmail = "geddy@fakemail.com",
                    IsConfirmed = false
                },
                new Friendship
                {
                    Id = 13,
                    RequesterEmail = "freddie@fakemail.com",
                    ConfirmerEmail = "pete@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 14,
                    RequesterEmail = "freddie@fakemail.com",
                    ConfirmerEmail = "roger@fakemail.com",
                    IsConfirmed = true
                },
                new Friendship
                {
                    Id = 15,
                    RequesterEmail = "geddy@fakemail.com",
                    ConfirmerEmail = "keith@fakemail.com",
                    IsConfirmed = false
                },
                new Friendship
                {
                    Id = 16,
                    RequesterEmail = "geddy@fakemail.com",
                    ConfirmerEmail = "freddie@fakemail.com",
                    IsConfirmed = false
                }
            );
        }
    }
}