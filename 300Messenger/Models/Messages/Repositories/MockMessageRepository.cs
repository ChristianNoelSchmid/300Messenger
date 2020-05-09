using System.Collections.Generic;
using System.Linq;

namespace _300Messenger.Models.Repositories
{
    public class MockMessageRepository : IMessageRepository
    {
        private List<Message> messages;
        public MockMessageRepository()
        {
            messages = new List<Message>();
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
            return messages.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<Message> GetMessagesForSession(int sessionId)
        {
            return messages.Where(m => m.MessageSessionId == sessionId);
        }
    } 
}