using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _300Messenger.Models
{
    public interface IMessageRepository
    { 
        Message CreateMessage(Message message);
        Message GetMessage(int id);
        IEnumerable<Message> GetMessages();
        Message UpdateMessage(Message message);
        Message DeleteMessage(int id);
        
    }
}
