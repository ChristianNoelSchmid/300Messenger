using System.Collections.Generic;
using System.Linq;

namespace _300Messenger.Models.Repositories
{
    /// <summary>
    /// A simple CRUD implementation of a Message repository
    /// stored In-Memory. Used during development
    /// </summary>
    public class MockMessageSessionRepository : IMessageSessionRepository
    {
        private List<MessageSession> sessions;

        public MockMessageSessionRepository()
        {
            sessions = new List<MessageSession>()
            {
                new MessageSession() { 
                    Id = 1, Title = "Lol, wtf?", Email = "csch**@**.com",
                    Messages = new List<Message>() {
                        new Message() { Type = MessageType.Text, Content = "WTF?" }
                    }
                },
                new MessageSession() { 
                    Id = 1, Title = "This is absurd", Email = "csch**@**.com",
                    Messages = new List<Message>() {
                        new Message() { Type = MessageType.Text, Content = "ABSURD!!" }
                    }
                }
            };
        }

        public MessageSession AddMessageToSession(int id, Message message)
        {
            var session = sessions.FirstOrDefault(m => m.Id == id);
            if(session != null)
            {
                session.Messages.Add(message);
            }

            return session;
        }

        public MessageSession CreateMessageSession(MessageSession message)
        {
            message.Id = sessions.Max(m => m.Id) + 1;
            sessions.Add(message);
            return message;
        }

        public MessageSession DeleteMessageSession(int id)
        {
            var message = sessions.FirstOrDefault(m => m.Id == id);
            if(message != null)
            {
                sessions.Remove(message);
            }

            return message;
        }

        public MessageSession GetMessageSession(int id)
        {
            var message = sessions.FirstOrDefault(m => m.Id == id);
            return message;
        }

        public IEnumerable<MessageSession> GetMessageSessions()
        {
            return sessions;
        }

        public MessageSession UpdateMessageSession(MessageSession message)
        {
            var oldMessage = sessions.FirstOrDefault(m => m.Id == message.Id);
            if(oldMessage != null)
            {
                oldMessage.Email = message.Email;
                oldMessage.Title = message.Title;
                oldMessage.Description = message.Description;
            }

            return message;
        }
    }
}