using System.Globalization;
using DrustveneMreze.Domain;

namespace DrustveneMreze.Repositories
{
    public class GroupRepository
    {

        private const string filePath = "Data/grupe.csv";
        public static Dictionary<int, Group> Data;

        public GroupRepository()
        {
            if (Data == null)
            {
                Load();
            }
        }

        private void Load()
        {
            Data = new Dictionary <int, Group>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] attributes = line.Split(',');
                int id = int.Parse(attributes[0]);
                string name = attributes[1];
                DateTime date = DateTime.ParseExact(attributes[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                Group group = new Group(id, name, date);
                Data[id] = group;

            }
        }

        public void Save()
        {
            List<string> lines = new List<string>();
            foreach (Group group in Data.Values)
            {
                lines.Add($"{group.Id},{group.Name},{group.Date:yyyy-MM-dd}");
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}
