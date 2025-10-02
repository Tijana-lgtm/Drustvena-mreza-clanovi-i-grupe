namespace DrustveneMreze.Domain
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName {  get; set; }
        public DateTime Incorporation {  get; set; }
        public List<User> Users { get; set; } = new List<User>();

        public Group(int id, string groupName, DateTime incorporation)
        {
            Id = id;
            GroupName = groupName;
            Incorporation = incorporation;
        }

        public Group() 
        {

        }
    }
}
