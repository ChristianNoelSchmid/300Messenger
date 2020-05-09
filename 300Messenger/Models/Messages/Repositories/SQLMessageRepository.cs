using System.Collections.Generic;
using System.Linq;

namespace _300Messenger.Models.Repositories
{
    public class SQLMessageRepository : IMessageRepository
    {
        private readonly AppDbContext dbContext;

        public SQLMessageRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Message CreateMessage(Message message)
        {
            dbContext.Messages.Add(message);
            dbContext.SaveChanges();

            return message;
        }

        public Message DeleteMessage(int id)
        {
            var message = dbContext.Messages.Find(id);
            if(message != null)
            {
                dbContext.Messages.Remove(message);
                dbContext.SaveChanges();
            }

            return message;
        }

        public Message GetMessage(int id)
        {
            return dbContext.Messages.Find(id);
        }

        public IEnumerable<Message> GetMessagesForSession(int sessionId)
        {
            return dbContext.Messages 
                .Where(m => m.MessageSessionId == sessionId);
        }
    }
}