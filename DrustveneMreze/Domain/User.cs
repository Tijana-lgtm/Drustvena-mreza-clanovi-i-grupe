
﻿using System.Text.Json.Serialization;

namespace DrustveneMreze.Domain

{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name {  get; set; }
        public string Surname {  get; set; }
        public DateTime BirthDate { get; set; }
        public List<Group>? Groups { get; set; } = null;
        public User(int id, string username, string name, string surname, DateTime birthDate)
        {
            Id = id;
            Username = username;
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
        }


        public User() 
        {
        }

    }
}
