using System.Globalization;
using DrustveneMreze.Domain;


namespace DrustveneMreze.Repositories

{
    public class GroupRepository
    {

        private const string filePath = "Data/grupe.csv";
        private const string membershipFilePath = "Data/clanstva.csv"; 
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
            if (File.Exists(membershipFilePath))
            {
                string[] membershipLines = File.ReadAllLines(membershipFilePath);
                UserRepository userRepository = new UserRepository(); 

                foreach (string line in membershipLines)
                {
                    string[] parts = line.Split(',');
                    int userId = int.Parse(parts[0]);
                    int groupId = int.Parse(parts[1]);

                    if (Data.ContainsKey(groupId) && UserRepository.Data.ContainsKey(userId))
                    {
                        Group group = Data[groupId];
                        User user = UserRepository.Data[userId];

                        group.Users.Add(user);

                    }
                }
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

            List<string> membershipLines = new List<string>();
            foreach (Group group in Data.Values)
            {
                foreach (User user in group.Users)
                {
                    membershipLines.Add($"{user.Id},{group.Id}");
                }
            }
            File.WriteAllLines(membershipFilePath, membershipLines);

        }
    }
}
