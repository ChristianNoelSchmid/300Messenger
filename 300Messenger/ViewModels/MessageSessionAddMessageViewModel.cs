using _300Messenger.Models;

namespace _300Messenger.ViewModels
{
    public class MessageSessionAddMessageViewModel
    {
        public MessageSession Session { get; set; }
        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}