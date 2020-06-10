using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Friendships.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Friendships.Models
{
    public class DbFriendshipRepo : IFriendshipRepo
    {
        private readonly AppDbContext dbContext;

        public DbFriendshipRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Friendship> AddUnconfirmedFriendshipAsync(string requesterEmail, string confirmerEmail)
        {
            var friendship = await dbContext.Friendships.FirstOrDefaultAsync(
                f => (f.RequesterEmail == requesterEmail || f.RequesterEmail == confirmerEmail) && 
                     (f.ConfirmerEmail == requesterEmail || f.ConfirmerEmail == confirmerEmail)
            );

            if(friendship != null)
            {
                throw new FriendshipAlreadyExistsException();
            }

            friendship = new Friendship() {
                RequesterEmail = requesterEmail,
                ConfirmerEmail = confirmerEmail,
                IsConfirmed = false
            };

            await dbContext.Friendships.AddAsync(
                friendship
            );
            await dbContext.SaveChangesAsync();

            return friendship;
        }

        public async Task<Friendship> ConfirmFriendshipAsync(int id, string confirmerEmail)
        {
            var friendship = await dbContext.Friendships.FindAsync(id);

            if(friendship == null)
                throw new FriendshipDoesNotExistException();
            if (friendship.ConfirmerEmail != confirmerEmail)
                throw new UnauthorizedFriendConfirmException();

            friendship.IsConfirmed = true;
            await dbContext.SaveChangesAsync();

            return friendship;
        }

        public IEnumerable<Friendship> GetAllFriendships(string fromEmail)
        {
            return dbContext.Friendships.Where(
                f => f.RequesterEmail == fromEmail || f.ConfirmerEmail == fromEmail
            );
        }

        public async Task<Friendship> GetFriendship(string fromEmail, string friendEmail)
        {
            var friendship = await dbContext.Friendships.FirstOrDefaultAsync(
                f => (f.RequesterEmail == fromEmail || f.ConfirmerEmail == fromEmail) &&
                     (f.RequesterEmail == friendEmail || f.ConfirmerEmail == friendEmail)
             );

            if (friendship == null) throw new FriendshipDoesNotExistException();
            return friendship;
        }

        public async Task<Friendship> RemoveFriendshipAsync(int id)
        {
            var friendship = await dbContext.Friendships.FindAsync(id);

            if(friendship == null)
            {
                throw new FriendshipDoesNotExistException();
            }
            
            dbContext.Friendships.Remove(friendship);
            await dbContext.SaveChangesAsync();

            return friendship;
        }
    }
}