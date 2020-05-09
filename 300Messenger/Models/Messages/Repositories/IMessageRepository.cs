using System.Collections.Generic;

namespace _300Messenger.Models.Repositories
{
    public interface IMessageRepository
    {
        Message CreateMessage(Message message);
        Message GetMessage(int id);
        IEnumerable<Message> GetMessagesForSession(int sessionId);
        Message DeleteMessage(int id);
    }
}