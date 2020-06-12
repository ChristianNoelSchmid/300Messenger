using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Friendships.Models
{
    /// <summary>
    /// Interface for Friendship Repository
    /// For dependency injection
    /// </summary>
    public interface IFriendshipRepo
    {
        Task<Friendship> AddUnconfirmedFriendshipAsync(string from, string to);
        Task<Friendship> RemoveFriendshipAsync(int id);
        Task<Friendship> ConfirmFriendshipAsync(int id, string confirmerEmail);
        Task<Friendship> GetFriendship(string fromEmail, string friendEmail);
        IEnumerable<Friendship> GetAllFriendships(string fromEmail);
    }
}