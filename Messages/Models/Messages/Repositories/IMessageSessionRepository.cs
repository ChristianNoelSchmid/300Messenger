using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Messages.Models.Repositories
{
    public interface IMessageSessionRepository
    { 
        Task<MessageSession> CreateMessageSessionAsync(MessageSession session);
        Task<MessageSession> GetMessageSessionAsync(int id, string requesterEmail);
        Task<MessageSession> DeleteMessageSessionAsync(int id, string requesterEmail);
        IEnumerable<MessageSession> GetMessageSessions(string email);
        Task<MessageSession> AddUsersToSessionAsync(int id, string requesterEmail, string[] userEmails);
        Task<MessageSession> AddMessageToSessionAsync(int sessionId, string requesterEmail, Message message);        
    }
}
