namespace DrustveneMreze.Domain
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();

        public User() { }
        public User (int id, string userName, string name, string surname, DateTime birthDate)   
        {
            Id = id;
            UserName = userName;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
        }
    }
}
