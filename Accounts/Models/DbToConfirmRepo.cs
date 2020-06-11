using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Models
{
    /// <summary>
    /// The database repository for the ToConfirms elements
    /// Allows adding and removing ToConfirms, as well as
    /// deleting old ToConfirms that have not been validated
    /// </summary>
    public class DbToConfirmRepo : IToConfirmRepo
    {
        // Microservice database context
        private readonly AppDbContext _context;
        public DbToConfirmRepo(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a ToConfirm to the database, representing
        /// a User's email that must be confirmed
        /// </summary>
        /// <param name="email">The User's email, which creates the unique token</param>
        /// <returns>The new ToConfirm</returns>
        public async Task<ToConfirm> AddAsync(string email)
        {
            var confirm = await _context.ToConfirms.FirstOrDefaultAsync(
                tc => tc.EmailToConfirm == email
            );
            if(confirm == null)
            {
                await _context.ToConfirms.AddAsync(
                    confirm = new ToConfirm()
                    {
                        EmailToConfirm = email,
                        Token = email.GetHashCode(), // The token is created via the email's hash code
                        TimeStamp = DateTime.Now
                    }
                ); 
            }
            await _context.SaveChangesAsync();
            return confirm;
        }

        /// <summary>
        /// Retrieves the ToConfirm by token
        /// </summary>
        public async Task<ToConfirm> GetToConfirmAsync(int token)
        {
            return await _context.ToConfirms.FirstOrDefaultAsync(
                tc => tc.Token == token
            );
        }

        /// <summary>
        /// Deletes all ToConfirms which are older than 30 days
        /// </summary>
        /// <returns></returns>
        public async Task DeleteAllMonthOldAsync()
        {
            var month = new TimeSpan(30, 0, 0, 0);
            await _context.ToConfirms.ForEachAsync(
                tc =>
                {
                    if(DateTime.Now - tc.TimeStamp > month)
                    {
                        _context.ToConfirms.Remove(tc);
                    }
                }
            );
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the ToConfirm by Id
        /// </summary>
        public async Task<ToConfirm> RemoveAsync(int id)
        {
            var toConfirm = await _context.ToConfirms.FindAsync(id);
            if(toConfirm != null)
            {
                _context.Remove(toConfirm);
                await _context.SaveChangesAsync();
            }

            return toConfirm;
        }
    }
}
