using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _300Messenger.Messages.Exceptions;

namespace _300Messenger.Messages.Models.Repositories
{
    /// <summary>
    /// Primary implementation of the Message repository.
    /// Stored via database (excluding images, which are stored
    /// on harddrive, with references in database).
    /// </summary>
    public class SQLMessageSessionRepository : IMessageSessionRepository
    {
        private readonly AppDbContext dbContext;

        public SQLMessageSessionRepository(AppDbContext context)
        {
            this.dbContext = context;
        }

        public async Task<MessageSession> AddMessageToSessionAsync(int sessionId, string requesterEmail, Message message)
        {
            var session = await dbContext.MessageSessions.FindAsync(sessionId);
            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }
                await dbContext.Messages.AddAsync(message);
                await dbContext.SaveChangesAsync();
            }

            return session;
        }

        public async Task<MessageSession> AddUsersToSessionAsync(int id, string requesterEmail, string[] userEmails)
        {
            var session = await dbContext.MessageSessions.FindAsync(id);
            if(session != null)
            {
                if(!session.Emails.StartsWith(requesterEmail))
                {
                    throw new EmailDoesNotMatchMessageSessionOwnerException();
                }
                foreach(var email in userEmails)
                {
                    session.Emails += $";{email}";
                }
                await dbContext.SaveChangesAsync();
            }

            return session;
        }

        public async Task<MessageSession> CreateMessageSessionAsync(MessageSession session)
        {
            await dbContext.MessageSessions.AddAsync(session);
            await dbContext.SaveChangesAsync();

            return session;
        }

        public async Task<MessageSession> DeleteMessageSessionAsync(int id, string requesterEmail)
        {
            var session = await dbContext.MessageSessions.FindAsync(id);
            if(session != null)
            {
                if(session.Emails.StartsWith(requesterEmail))
                {
                    dbContext.MessageSessions.Remove(session);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new EmailDoesNotMatchMessageSessionOwnerException();
                }
            }

            return session;
        }

        public async Task<MessageSession> GetMessageSessionAsync(int id, string requesterEmail)
        {
            var session = await dbContext.MessageSessions.FindAsync(id);
            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                } 
            }

            return session;
        }

        public IEnumerable<MessageSession> GetMessageSessions(string email)
        {
            return dbContext.MessageSessions.Where(
                ms =>  ms.Emails.Contains(email)
            );
        }

        public async Task<IEnumerable<Message>> GetMessagesForSessionAsync(int id, string requesterEmail)
        {
            var session = await dbContext.MessageSessions.FindAsync(id);

            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }
                return  dbContext.Messages.Where(m => m.MessageSessionId == id);
            }

            return null;
        }

        public async Task<MessageSession> RemoveUsersFromSessionAsync(int id, string requesterEmail, string[] usersToRemoveEmails)
        {
            var session = await dbContext.MessageSessions.FindAsync(id);   
            if(session != null)
            {
                if(!session.Emails.StartsWith(requesterEmail))
                {
                    throw new EmailDoesNotMatchMessageSessionOwnerException();
                }
                
                var emailList = new List<string>(session.Emails.Split(';'));
                foreach(var userEmail in usersToRemoveEmails)
                {
                    emailList.Remove(userEmail);
                }
                session.Emails = string.Join(';', emailList);
                await dbContext.SaveChangesAsync();
            }

            return session;
        }

        public async Task<string[]> GetSessionUsersAsync(int id, string requesterEmail)
        {
            var session = await dbContext.MessageSessions.FindAsync(id); 
            if(session != null)
            {
                if(!session.Emails.Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }

                return session.Emails.Split(';');
            }

            return null;
        }
    }
}
