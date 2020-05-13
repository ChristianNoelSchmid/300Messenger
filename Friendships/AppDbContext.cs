using _300Messenger.Friendships.Models;
using Microsoft.EntityFrameworkCore;

namespace _300Messenger.Friendships
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Friendship> Friendships { get; set; }
    }
}