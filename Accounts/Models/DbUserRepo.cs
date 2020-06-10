using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounts.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Accounts.Models
{
    public class DbUserRepo : IUserRepo
    {
        private readonly AppDbContext dbContext;
        private readonly PasswordHasher<User> hasher;

        public DbUserRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.hasher = new PasswordHasher<User>();
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            user.Email = user.Email.ToLower();
            if(await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email) != null)
            {
                throw new UserAlreadyExistsException();
            }

            user.PasswordHash = hasher.HashPassword(user, password);

            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> LoginUserAsync(string email, string password)
        {
            email = email.ToLower();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if(user == null)
            {
                throw new UserDoesNotExistException();
            }

            var pwResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if(pwResult == PasswordVerificationResult.Failed)
            {
                throw new UserPasswordDoesNotMatchException();
            }

            return user;
        }

        public async Task<User> UpdateUserInfoAsync(User user)
        {
            user.Email = user.Email.ToLower();
            var userOld = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

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

            await dbContext.SaveChangesAsync();
            return userOld;
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null) 
            {
                throw new UserDoesNotExistException();
            }

            return user;
        }

        public IEnumerable<User> Where(Func<User, bool> pred)
        {
            return dbContext.Users.Where(pred);
        }

        public async Task<User> RemoveUserAsync(string email)
        {
            email = email.ToLower();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if(user == null)
            {
                throw new UserDoesNotExistException();
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return user;
        }
    }
}