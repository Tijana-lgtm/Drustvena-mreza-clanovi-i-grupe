namespace DrustveneMreze.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date {  get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }


        public Post() { }

        public Post (int id, string content, DateTime date)
        {
            Id = id;
            Content = content;
            Date = date;
        }
    }
}
