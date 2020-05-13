using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Friendships.Models
{
    public class FriendshipRepo : IFriendshipRepo
    {
        private readonly AppDbContext dbContext;

        public FriendshipRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Friendship> AddUnconfirmedFriendship(string from, string to)
        {
            var friendship = new Friendship() {
                FromEmail = from,
                ToEmail = to,
                IsConfirmed = false
            };

            await dbContext.Friendships.AddAsync(
                friendship
            );
            await dbContext.SaveChangesAsync();

            return friendship;
        }

        public async Task<Friendship> ConfirmFriendship(int id)
        {
            var friendship = await dbContext.Friendships.FindAsync(id);
            if(friendship != null)
            {
                friendship.IsConfirmed = true;
                await dbContext.SaveChangesAsync();
            }

            return friendship;
        }

        public IEnumerable<Friendship> GetAllConfirmedFriendships(string fromEmail)
        {
            return dbContext.Friendships.Where(
                f => (f.FromEmail == fromEmail || f.ToEmail == fromEmail) && f.IsConfirmed 
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