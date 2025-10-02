namespace DrustveneMreze.Domain
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public List<User> Users { get; set; } = new List<User>();

        public Group() { }
        public Group(int id, string name, DateTime date)
        {
            Id = id;
            Name = name;
            Date = date;
        }
    }
}
