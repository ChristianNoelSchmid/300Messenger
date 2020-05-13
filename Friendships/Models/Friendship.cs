namespace _300Messenger.Friendships.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public bool IsConfirmed { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
    }
}