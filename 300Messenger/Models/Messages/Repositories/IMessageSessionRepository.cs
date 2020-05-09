using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Models.Repositories
{
    public interface IMessageSessionRepository
    { 
        MessageSession CreateMessageSession(MessageSession message);
        MessageSession GetMessageSession(int id);
        IEnumerable<MessageSession> GetMessageSessions();
        MessageSession UpdateMessageSession(MessageSession message);
        MessageSession DeleteMessageSession(int id);

        MessageSession AddMessageToSession(int id, Message message);
        
    }
}
