using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Messages.Models.Repositories
{
    public interface IMessageSessionRepository
    { 
        Task<MessageSession> GetMessageSessionAsync(int id, string requesterEmail);
        IEnumerable<MessageSession> GetMessageSessions(string email);
        Task<MessageSession> CreateMessageSessionAsync(MessageSession session);
        Task<MessageSession> DeleteMessageSessionAsync(int id, string requesterEmail);
        Task<MessageSession> AddUsersToSessionAsync(int id, string requesterEmail, string[] userEmails);
        Task<MessageSession> AddMessageToSessionAsync(int sessionId, string requesterEmail, Message message);        
        Task<IEnumerable<Message>> GetMessagesForSessionAsync(int id, string requesterEmail);
        Task<MessageSession> RemoveUsersFromSessionAsync(int id, string requesterEmail, string[] userToRemoveEmail);
        Task<string[]> GetSessionUsersAsync(int id, string requesterEmail);
    }
}
