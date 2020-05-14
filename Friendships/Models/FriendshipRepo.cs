using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace _300Messenger.Friendships.Models
{
    public class FriendshipRepo : IFriendshipRepo
    {
        private readonly AppDbContext dbContext;

        public FriendshipRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Friendship> AddUnconfirmedFriendship(string requesterEmail, string confirmerEmail)
        {
            var friendship = new Friendship() {
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

        public async Task<Friendship> ConfirmFriendship(string requesterEmail, string confirmerEmail)
        {
            var friendship = await dbContext.Friendships
                .FirstOrDefaultAsync(
                    f => 
                    f.RequesterEmail == requesterEmail && f.ConfirmerEmail == confirmerEmail
                );

            if(friendship != null)
            {
                friendship.IsConfirmed = true;
                await dbContext.SaveChangesAsync();
            }

            return friendship;
        }

        public IEnumerable<Friendship> GetAllFriendships(string fromEmail)
        {
            return dbContext.Friendships.Where(
                f => f.RequesterEmail == fromEmail || f.ConfirmerEmail == fromEmail
            );
        }

        public async Task<Friendship> RemoveFriendship(int id)
        {
            var friendship = await dbContext.Friendships.FindAsync(id);
            if(friendship != null)
            {
                dbContext.Friendships.Remove(friendship);
                await dbContext.SaveChangesAsync();
            }

            return friendship;
        }
    }
}