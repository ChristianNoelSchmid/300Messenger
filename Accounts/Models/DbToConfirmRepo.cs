using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Models
{
    public class DbToConfirmRepo : IToConfirmRepo
    {
        private readonly AppDbContext _context;
        public DbToConfirmRepo(AppDbContext context)
        {
            _context = context;
        }

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
                        Token = email.GetHashCode(),
                        TimeStamp = DateTime.Now
                    }
                ); 
            }
            await _context.SaveChangesAsync();
            return confirm;
        }

        public async Task<ToConfirm> GetToConfirmAsync(int token)
        {
            return await _context.ToConfirms.FirstOrDefaultAsync(
                tc => tc.Token == token
            );
        }

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
