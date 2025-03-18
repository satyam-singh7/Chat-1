namespace Chat.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public string Message { get; set; }
        public string MediaUrl { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string GroupName { get; set; }
       
    }
}
