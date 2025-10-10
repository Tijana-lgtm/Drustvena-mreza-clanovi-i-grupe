using System.Globalization;
using System.Runtime.CompilerServices;
using DrustveneMreze.Domain;
using Microsoft.Data.Sqlite;

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

    public class UserDbRepository
    {
        private readonly string connectionString;

        public UserDbRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }
        public List<User> GetAll()
        {
            List<User> users = new List<User>();       

            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "SELECT Id, Username, Name, Surname, Birthday FROM Users";

                using SqliteCommand command = new SqliteCommand(query, connection);
                using SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    User user = new User()
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        Name = reader["Name"].ToString(),
                        Surname = reader["Surname"].ToString(),
                        BirthDate = DateTime.Parse(reader["Birthday"].ToString())

                    };
                    users.Add(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska pri citanju iz baze " + ex.Message);
                throw;
            }


            return users;
        }

        public User GetById(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();
                string query = "SELECT Id, Username, Name, Surname, Birthday FROM Users WHERE Id = @id";
                using SqliteCommand command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);
                using SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Username = reader["Username"].ToString(),
                        Name = reader["Name"].ToString(),
                        Surname = reader["Surname"].ToString(),
                        BirthDate = DateTime.Parse(reader["Birthday"].ToString()),
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Greska pri citanju korisnika: " + ex.Message);
            }
            return null;
        }

        public User CreateUser(User user)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "iNSERT INTO Users (Username, Name, Surname, Birthday) VALUES (@username, @name, @surname, @birthday); " + "SELECT LAST_INSERT_ROWID();";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@birthday", user.BirthDate.ToString("yyyy-MM-dd"));

                int lastId = Convert.ToInt32(command.ExecuteScalar());
                Console.WriteLine($"Id korisnika koji je poslednji unet je: {lastId}");
                user.Id = lastId;
                return user;
            }
            catch(Exception ex) 
            {
                Console.WriteLine("Greska pri kreiranju korisnika: " + ex.Message);
                return null;
            }
        }

        public bool UpdateUser(int id, User user)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "UPDATE Users SET Username = @username, Name = @name, Surname = @surname, Birthday = @Birthday WHERE Id = @id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@birthday", user.BirthDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@id", id);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Greska pri azuriranju korisnika: " + ex.Message);
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM Users WHERE Id = @id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch( Exception ex )
            {
                Console.WriteLine("Greska pri brisanju korisnika: " + ex.Message);
                return false;
            }
        }
    }
}

