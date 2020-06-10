using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounts.Models
{
    public interface IUserRepo
    {
        Task<User> CreateUserAsync(User user, string password);
        Task<User> LoginUserAsync(string email, string password);
        Task<User> UpdateUserInfoAsync(User user);
        Task<User> RemoveUserAsync(string email);
        Task<User> GetUserAsync(string email);
        IEnumerable<User> Where(Func<User, bool> pred);
    }
}