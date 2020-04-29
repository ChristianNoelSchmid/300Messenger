using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Models
{
    public class SQLMessageRepository : IMessageRepository
    {
        private readonly AppDbContext context;

        public SQLMessageRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Message CreateMessage(Message message)
        {
            context.Messages.Add(message);
            context.SaveChanges();

            return message;
        }

        public Message DeleteMessage(int id)
        {
            var message = context.Messages.Find(id);
            if (message != null)
            {
                context.Messages.Remove(message);
                context.SaveChanges();
            }

            return message;
        }

        public Message GetMessage(int id)
        {
            return context.Messages.Find(id);
        }

        public IEnumerable<Message> GetMessages()
        {
            return context.Messages;
        }

        public Message UpdateMessage(Message message)
        {
            var oldMessage = context.Messages.FirstOrDefault(m => m.Id == message.Id);
            if(oldMessage != null)
            {
                oldMessage.Email = message.Email;
                oldMessage.Content = message.Content;
            }

            return message;
        }
    }
}
