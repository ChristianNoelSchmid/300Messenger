using Accounts.Exceptions;
using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accounts.Models
{
    public class MockUserRepo : IUserRepo
    {
        private PasswordHasher<User> _hasher = new PasswordHasher<User>();
        private List<User> _users;

        public MockUserRepo()
        {
            _users = new List<User>(
                new User[]
                {
                    new User
                    {
                        Id = 1,
                        Email = "roger@fakemail.com",
                        EmailConfirmed = true,
                        FirstName = "Roger",
                        LastName = "Daltry",
                    },
                    new User
                    {
                        Id = 2,
                        Email = "keith@fakemail.com",
                        EmailConfirmed = false,
                        FirstName = "Keith",
                        LastName = "Moon",
                    },
                    new User
                    {
                        Id = 3,
                        Email = "geddy@fakemail.com",
                        EmailConfirmed = false,
                        FirstName = "Geddy",
                        LastName = "Lee",
                    }
                }
            );

            foreach(var user in _users)
            {
                user.PasswordHash = _hasher.HashPassword(user, "password");
            }
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            user.Email = user.Email.ToLower();
            if (_users.Any(u => u.Email == user.Email))
                throw new UserAlreadyExistsException();

            user.PasswordHash = _hasher.HashPassword(user, password);
            _users.Add(user);

            return await Task.Run(() => user);
        }

        public async Task<User> GetUserAsync(string email)
        {
            email = email.ToLower();
            return await Task.Run(() => _users.FirstOrDefault(u => u.Email == email));
        }

        public async Task<User> LoginUserAsync(string email, string password)
        {
            email = email.ToLower();
            var user = _users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                throw new UserDoesNotExistException();

            var verifyPassword = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verifyPassword == PasswordVerificationResult.Failed)
                throw new UserPasswordDoesNotMatchException();

            return await Task.Run(() => user);
        }

        public async Task<User> RemoveUserAsync(string email)
        {
            email = email.ToLower();
            var user = _users.FirstOrDefault(u => u.Email == email);

            if (user == null)
                throw new UserDoesNotExistException();

            _users.Remove(user);
            return await Task.Run(() => user);
        }

        public async Task<User> UpdateUserInfoAsync(User user)
        {
            user.Email = user.Email.ToLower();
            var userOld = _users.FirstOrDefault(u => u.Email == user.Email);

            if(userOld == null)
            {
                throw new UserDoesNotExistException();
            }
            userOld = new User()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailConfirmed = user.EmailConfirmed,
                PasswordHash = user.PasswordHash
            };

            return await Task.Run(() => userOld);
        }

        public IEnumerable<User> Where(Func<User, bool> pred)
        {
            return _users.Where(pred);
        }
    }
}
