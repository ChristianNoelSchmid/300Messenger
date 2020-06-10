using Messages.Exceptions;
using Messages.ViewModels;
using Shared.Models;
using Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Messages.Models.Repositories
{
    public class MockMessageSessionRepo : IMessageSessionRepository
    {
        private List<MessageSession> _sessions;
        private List<Message> _messages;

        public MockMessageSessionRepo()
        {
            _sessions = new List<MessageSession>();
            _messages = new List<Message>();
            _sessions.Add(new MessageSession
            {
                Id = 1,
                Title = "Beach",
                Description = "Let's go to the beach",
                Emails = "geddy@fakemail.com;alex@fakemail.com;neil@fakemail.com",
            });
            _messages.Add(new Message
            {
                Id = 1,
                Email = "geddy@fakemail.com",
                Content = "Food?",
                MessageSessionId = 1,
                TimeStamp = DateTime.Now,
                Type = MessageType.Text
            });
            _messages.Add(new Message
            {
                Id = 2,
                Email = "alex@fakemail.com",
                Content = "NO!",
                MessageSessionId = 1,
                TimeStamp = DateTime.Now,
                Type = MessageType.Text
            });

            _sessions.Add(new MessageSession
            {
                Id = 2,
                Title = "Pizza",
                Description = "Pizza toppings?",
                Emails = "roger@fakemail.com;pete@fakemail.com;keith@fakemail.com",
            });
            _messages.Add(new Message
            {
                Id = 3,
                Email = "roger@fakemail.com",
                Content = "Pepperoni!",
                MessageSessionId = 2,
                TimeStamp = DateTime.Now,
                Type = MessageType.Text
            });
            _messages.Add(new Message
            {
                Id = 4,
                Email = "pete@fakemail.com",
                Content = "NOTHING!",
                MessageSessionId = 2,
                TimeStamp = DateTime.Now,
                Type = MessageType.Text
            });
        }

        public async Task<MessageSession> GetMessageSessionAsync(int id, string requesterEmail)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                } 
            }

            return await Task.Run(() => session);
        }
        public IEnumerable<MessageSession> GetMessageSessions(string email)
        {
            return _sessions.Where(
                ms => ms.Emails.Contains(email)
            );
        }
        public async Task<MessageSession> CreateMessageSessionAsync(MessageSession session)
        {
            _sessions.Add(session);
            return await Task.Run(() => session);
        }
        public async Task<MessageSession> DeleteMessageSessionAsync(int id, string requesterEmail)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            if(session != null)
            {
                if(session.Emails.StartsWith(requesterEmail))
                {
                    _sessions.Remove(session);
                }
                else
                {
                    throw new EmailDoesNotMatchMessageSessionOwnerException();
                }
            }

            return await Task.Run(() => session);
        }
        public async Task<MessageSession> UpdateMessageSessionAsync(int id, MessageSessionCreateViewModel newSession)
        {
            throw new NotImplementedException();
        }

        public async Task<MessageSession> AddMessageToSessionAsync(int sessionId, string requesterEmail, Message message)
        {
            if (message.MessageSessionId != sessionId)
                throw new EmailNotAssociatedWithMessageSessionException();

            var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }
                _messages.Add(message);
            }

            return await Task.Run(() => session);
        }
        public async Task<IEnumerable<Message>> GetMessagesForSessionAsync(int id, string requesterEmail)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);

            if(session != null)
            {
                if(!session.Emails.Split(';').Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }
                return await Task.Run(() =>
                    _messages
                    .Where(m => m.MessageSessionId == id)
                    .OrderBy(m => m.TimeStamp)
                );
            }

            return null;
        }

        public async Task<string[]> GetSessionUsersAsync(int id, string requesterEmail)
        {
            var session = _sessions.FirstOrDefault(s => s.Id == id);
            if(session != null)
            {
                if(!session.Emails.Contains(requesterEmail))
                {
                    throw new EmailNotAssociatedWithMessageSessionException();
                }

                return await Task.Run(() => session.Emails.Split(';'));
            }

            return null;
        }
    }
}
