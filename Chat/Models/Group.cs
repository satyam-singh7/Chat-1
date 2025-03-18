namespace Chat.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminId { get; set; }
        public List<User> Members { get; set; } = new List<User>();
    }
}
