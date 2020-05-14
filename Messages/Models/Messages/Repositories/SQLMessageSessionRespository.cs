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
                session.Messages.Add(message);
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
            session.Messages = new List<Message>(dbContext.Messages.Where(m => m.MessageSessionId == id));

            return session;
        }

        public IEnumerable<MessageSession> GetMessageSessions(string email)
        {
            return dbContext.MessageSessions.Where(
                ms => ms.Emails.Split().Contains(email)
            );
        }
    }
}
