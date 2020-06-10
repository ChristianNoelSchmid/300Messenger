namespace Shared.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public bool IsConfirmed { get; set; }
        public string RequesterEmail { get; set; }
        public string ConfirmerEmail { get; set; }
    }
}