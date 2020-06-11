using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Accounts.Models
{
    /// <summary>
    /// The database context for the microservice
    /// Connects with the Users and ToConfirms database
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<ToConfirm> ToConfirms { get; set; }
    }
}