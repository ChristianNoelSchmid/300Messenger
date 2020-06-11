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
    /// <summary>
    /// The database repository for the User elements
    /// Allows adding and removing Users, and handles
    /// Retrieval (ie. logging in) of Users
    /// </summary>
    public class DbUserRepo : IUserRepo
    {
        private readonly AppDbContext dbContext;

        /// <summary>
        /// The password hasher - allows passwords to be
        /// stored in the database securely
        /// </summary>
        private readonly PasswordHasher<User> hasher;

        public DbUserRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            hasher = new PasswordHasher<User>();
        }

        /// <summary>
        /// Creates a new User, throwing exceptions if the User already
        /// exists (by email)
        /// </summary>
        /// <param name="user">The new User</param>
        /// <param name="password">The User's unhashed password</param>
        /// <returns></returns>
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

        /// <summary>
        /// Logs in the User.
        /// Because this is a microservice, this is quite similar to
        /// GetUser. However, logging in the User requires an email and password.
        /// </summary>
        /// <param name="email">The supplied email address</param>
        /// <param name="password">The User's unhashed password</param>
        /// <returns></returns>
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

        /// <summary>
        /// Updates the User's information
        /// It is assumed that security is handled in this case in the
        /// Controller, since there is no email or password provided (ie. Jwt's)
        /// </summary>
        /// <param name="user">The User being updated, along with it's updated information</param>
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

        /// <summary>
        /// Gets the User, using supplied email
        /// It is assumed that security is handled in this case in the
        /// Controller, since there is no email or password provided (ie. Jwt's)
        /// </summary>
        /// <param name="email">The email of the User</param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string email)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null) 
            {
                throw new UserDoesNotExistException();
            }

            return user;
        }

        /// <summary>
        /// Retrieves all Users by the supplied predicate.
        /// </summary>
        /// <param name="pred">The predicate that performs the search</param>
        public IEnumerable<User> Where(Func<User, bool> pred)
        {
            return dbContext.Users.Where(pred);
        }

        /// <summary>
        /// Removes the User, by email, from the database
        /// </summary>
        /// <param name="email">The User's email</param>
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