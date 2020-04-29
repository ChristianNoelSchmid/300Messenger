using System.Collections.Generic;
using System.Linq;

namespace _300Messenger.Models
{
    public class MockMessageRepository : IMessageRepository
    {
        private List<Message> messages;

        public MockMessageRepository()
        {
            messages = new List<Message>()
            {
                new Message() { Id = 1, Content = "Lol, wtf?", Email = "csch**@**.com" },
                new Message() { Id = 2, Content = "I know, right?", Email = "aschm**@**.com" },
                new Message() { Id = 3, Content = "I'm a corgi!", Email = "corgi@**.com" },
            };
        }

        public Message CreateMessage(Message message)
        {
            message.Id = messages.Max(m => m.Id) + 1;
            messages.Add(message);
            return message;
        }

        public Message DeleteMessage(int id)
        {
            var message = messages.FirstOrDefault(m => m.Id == id);
            if(message != null)
            {
                messages.Remove(message);
            }

            return message;
        }

        public Message GetMessage(int id)
        {
            var message = messages.FirstOrDefault(m => m.Id == id);
            return message;
        }

        public IEnumerable<Message> GetMessages()
        {
            return messages;
        }

        public Message UpdateMessage(Message message)
        {
            var oldMessage = messages.FirstOrDefault(m => m.Id == message.Id);
            if(oldMessage != null)
            {
                oldMessage.Content = message.Content;
                oldMessage.Email = message.Email;
            }

            return message;
        }
    }
}