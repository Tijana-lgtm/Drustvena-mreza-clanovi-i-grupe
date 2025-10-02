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
            Data = new Dictionary<int, Group>();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] attributes = line.Split(',');
                int id = int.Parse(attributes[0]);
                string groupName = attributes[1];
                DateTime incorporation = DateTime.ParseExact(attributes[2], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                Group group = new Group(id, groupName, incorporation);
                Data[id] = group;
            }
        }

        public void Save()
        {
            List<string> groupLines = new List<string>();
            foreach (Group group in Data.Values)
            {
                groupLines.Add($"{group.Id},{group.GroupName},{group.Incorporation:yyyy-MM-dd}");
            }
            File.WriteAllLines(filePath, groupLines);
        }
    }
}
