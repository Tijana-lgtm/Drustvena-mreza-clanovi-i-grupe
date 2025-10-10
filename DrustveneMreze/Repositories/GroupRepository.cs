using System.Globalization;
using DrustveneMreze.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;


namespace DrustveneMreze.Repositories

{
    public class GroupRepository
    {

        private const string filePath = "Data/grupe.csv";
        private const string clanstvaPath = "Data/clanstva.csv";

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

            if (File.Exists(clanstvaPath))
            {
                string[] linesClanstva = File.ReadAllLines(clanstvaPath);
                UserRepository userRepository = new UserRepository();

                foreach (string line in linesClanstva)

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
            File.WriteAllLines(clanstvaPath, membershipLines);
        }
    }
    public class GroupDbRepository
    {
        private readonly string connectionString;

        public GroupDbRepository (IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }
        public Group GetById(int id)
        {
            Group group = null;

            try
            {
                SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "SELECT Id, Name, CreationDate FROM Groups WHERE Id = @Id";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            group = new Group();
                            group.Id = reader.GetInt32(0);
                            group.GroupName = reader.GetString(1);
                            group.Incorporation = DateTime.Parse(reader.GetString(2));
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u formatu podataka: {ex.Message}");
                throw;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili SQL upitu: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Pogresna operacija nad konekcijom ili komandama: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }

            return group;
        }
        public List<Group> GetAll(int page, int pageSize)
        {
            List<Group> groups = new List<Group>();

            try
            {
                SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = @"SELECT Id, Name, CreationDate FROM Groups
                         LIMIT @PageSize OFFSET @Offset";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PageSize", pageSize);
                    command.Parameters.AddWithValue("@Offset", pageSize * (page - 1));

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Group group = new Group();
                            group.Id = reader.GetInt32(0);
                            group.GroupName = reader.GetString(1);
                            group.Incorporation = DateTime.Parse(reader.GetString(2));
                            groups.Add(group);
                        }
                    }
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greska u formatu podataka: {ex.Message}");
                throw;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili SQL upitu: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Pogresna operacija nad konekcijom ili komandama: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
                throw;
            }

            return groups;
        }
        public int CountAll()
        {
            int count = 0;
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "SELECT COUNT(*) FROM Groups";

                using SqliteCommand command = new SqliteCommand(query, connection);
                count = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error counting groups: {ex.Message}");
                throw;
            }
            return count;
        }

        public int Create(Group newGroup)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = @"INSERT INTO Groups (Name, CreationDate) VALUES (@Name, @CreationDate); 
                         SELECT LAST_INSERT_ROWID();";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", newGroup.GroupName);
                command.Parameters.AddWithValue("@CreationDate", newGroup.Incorporation.ToString("yyyy-MM-dd"));

                int newId = Convert.ToInt32(command.ExecuteScalar());
                return newId;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri povezivanju sa bazom ili izvršavanju SQL upita: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Greška u formatu podataka: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Greška jer konekcija nije ili je više puta otvorena: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public bool Update(Group updatedGroup)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "UPDATE Groups SET Name = @Name, CreationDate = @CreationDate WHERE Id = @Id";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Name", updatedGroup.GroupName);
                command.Parameters.AddWithValue("@CreationDate", updatedGroup.Incorporation.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Id", updatedGroup.Id);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri povezivanju sa bazom ili izvršavanju SQL upita: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Greška jer konekcija nije ili je više puta otvorena: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM Groups WHERE Id = @Id";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greška pri povezivanju sa bazom ili izvršavanju SQL upita: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Greška jer konekcija nije ili je više puta otvorena: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neočekivana greška: {ex.Message}");
                throw;
            }
        }
    }
}
