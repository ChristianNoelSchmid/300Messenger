using System.Collections.Generic;
using System.Threading.Tasks;

namespace _300Messenger.Friendships.Models
{
    public interface IFriendshipRepo
    {
        Task<Friendship> AddUnconfirmedFriendship(string from, string to);
        Task<Friendship> RemoveFriendship(int id);
        Task<Friendship> ConfirmFriendship(int id);
        IEnumerable<Friendship> GetAllConfirmedFriendships(string from);
    }
}