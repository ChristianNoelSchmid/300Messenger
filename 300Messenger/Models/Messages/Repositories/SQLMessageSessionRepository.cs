using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Models.Repositories
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

        public MessageSession AddMessageToSession(int id, Message message)
        {
            var session = GetMessageSession(id);
            if(session != null)
            {
                session.Messages.Add(message);
                dbContext.SaveChanges();
            }

            return session;
        }

        public MessageSession CreateMessageSession(MessageSession message)
        {
            dbContext.MessageSessions.Add(message);
            dbContext.SaveChanges();

            return message;
        }

        public MessageSession DeleteMessageSession(int id)
        {
            var message = dbContext.MessageSessions.Find(id);
            if (message != null)
            {
                dbContext.MessageSessions.Remove(message);
                dbContext.SaveChanges();
            }

            return message;
        }

        public MessageSession GetMessageSession(int id)
        {
            return dbContext.MessageSessions.Find(id);
        }

        public IEnumerable<MessageSession> GetMessageSessions()
        {
            return dbContext.MessageSessions;
        }

        public MessageSession UpdateMessageSession(MessageSession message)
        {
            var oldMessage = dbContext.MessageSessions.FirstOrDefault(m => m.Id == message.Id);
            if(oldMessage != null)
            {
                oldMessage.Email = message.Email;
                oldMessage.Title = message.Title;
                oldMessage.Description = message.Description;
            }

            dbContext.SaveChanges();

            return message;
        }
    }
}
