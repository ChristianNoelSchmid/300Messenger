using System.Collections.Generic;
using System.Threading.Tasks;

namespace _300Messenger.Friendships.Models
{
    public interface IFriendshipRepo
    {
        Task<Friendship> AddUnconfirmedFriendship(string from, string to);
        Task<Friendship> RemoveFriendship(int id);
        Task<Friendship> ConfirmFriendship(string requesterEmail, string confirmerEmail);
        IEnumerable<Friendship> GetAllFriendships(string fromEmail);
    }
}