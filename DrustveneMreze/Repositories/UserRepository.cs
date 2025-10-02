using System.Globalization;
using DrustveneMreze.Domain;

namespace DrustveneMreze.Repositories
{
    public class UserRepository
    {

        private const string filePath = "Data/korisnici.csv";
        public static Dictionary<int, User> Data;

        public UserRepository()
        {
            if (Data == null)
            {
                Load();
            }
        }

        private void Load()
        {
            Data = new Dictionary<int, User>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] attributes = line.Split(',');
                int id = int.Parse(attributes[0]);

                string username = attributes[1];
                string name = attributes[2];
                string surname = attributes[3];
                DateTime birthDate = DateTime.ParseExact(attributes[4], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                User user = new User(id, username, name, surname, birthDate);

                Data[id] = user;

            }
        }

        public void Save()
        {
            List<string> lines = new List<string>();
            foreach (User user in Data.Values)
            {

                lines.Add($"{user.Id},{user.Username},{user.Name},{user.Surname},{user.BirthDate:yyyy-MM-dd}");
            }
            File.WriteAllLines(filePath, lines);
        }


    }
}
