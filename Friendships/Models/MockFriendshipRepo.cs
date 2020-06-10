using Friendships.Exceptions;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Friendships.Models
{
    public class MockFriendshipRepo : IFriendshipRepo
    {
        private List<Friendship> _friendships;
        
        public MockFriendshipRepo()
        {
            _friendships = new List<Friendship>();
            _friendships.Add(new Friendship
            {
                Id = 1,
                ConfirmerEmail = "geddy@fakemail.com",
                RequesterEmail = "alex@fakemail.com",
                IsConfirmed = true
            });
            _friendships.Add(new Friendship
            {
                Id = 2,
                ConfirmerEmail = "geddy@fakemail.com",
                RequesterEmail = "neil@fakemail.com",
                IsConfirmed = false
            });
        }

        public async Task<Friendship> AddUnconfirmedFriendshipAsync(string requesterEmail, string confirmerEmail)
        {
            var friendship = _friendships.FirstOrDefault(
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

            _friendships.Add(friendship);

            return await Task.Run(() => friendship);
        }

        public async Task<Friendship> RemoveFriendshipAsync(int id)
        {
            var friendship = _friendships.FirstOrDefault(f => f.Id == id);

            if(friendship == null)
            {
                throw new FriendshipDoesNotExistException();
            }
            
            _friendships.Remove(friendship);

            return await Task.Run(() => friendship);
        }

        public async Task<Friendship> ConfirmFriendshipAsync(int id, string confirmerEmail)
        {
            var friendship = _friendships.FirstOrDefault(f => f.Id == id);

            if(friendship == null)
                throw new FriendshipDoesNotExistException();
            if (friendship.ConfirmerEmail != confirmerEmail)
                throw new UnauthorizedFriendConfirmException();

            friendship.IsConfirmed = true;
            return await Task.Run(() => friendship);
        }

        public async Task<Friendship> GetFriendship(string fromEmail, string friendEmail)
        {
            var friendship = _friendships.FirstOrDefault(
                f => (f.RequesterEmail == fromEmail || f.ConfirmerEmail == fromEmail) &&
                     (f.RequesterEmail == friendEmail || f.ConfirmerEmail == friendEmail)
             );

            if (friendship == null) throw new FriendshipDoesNotExistException();
            return await Task.Run(() => friendship);
        }
        public IEnumerable<Friendship> GetAllFriendships(string fromEmail)
        {
            return _friendships.Where(
                f => f.RequesterEmail == fromEmail || f.ConfirmerEmail == fromEmail
            );
        }
    }
}