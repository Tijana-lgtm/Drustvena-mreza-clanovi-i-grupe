using DrustveneMreze.Domain;
using Microsoft.Data.Sqlite;

namespace DrustveneMreze.Repositories
{
    public class PostDbRepository
    {
        private readonly string connectionString;

        public PostDbRepository(IConfiguration configuration)
        {
            connectionString = configuration["ConnectionString:SQLiteConnection"];
        }

        public List<Post> GetAll()
        {
            List<Post> posts = new List<Post>();

            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT p.Id as PostId, p.UserId as PostUserId, p.Content, p.Date,
                        u.Id as UserId, u.Username, u.Name, u.Surname, u.Birthday
                        FROM Posts p
                        INNER JOIN Users u ON p.UserId = u.Id
                        ORDER BY p.Date DESC";

                    using (SqliteCommand command = new SqliteCommand(query, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Post post = new Post
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("PostUserId")),
                                    Content = reader.GetString(reader.GetOrdinal("Content")),

                                    Date = DateTime.Parse(reader.GetString(reader.GetOrdinal("Date"))),

                                    User = new User
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        Username = reader.GetString(reader.GetOrdinal("Username")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                        BirthDate = DateTime.Parse(reader.GetString(reader.GetOrdinal("Birthday")))
                                    }
                                };
                                posts.Add(post);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching posts: {ex.Message}");
                throw new Exception("An error occurred while fetching posts.", ex);
            }
            return posts;
        }
        public Post GetById(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = @"
                    SELECT p.Id as PostId, p.UserId, p.Content, p.Date,
                    u.Id as UserId, u.Username, u.Name, u.Surname, u.Birthday
                    FROM Posts p
                    INNER JOIN Users u ON p.UserId = u.Id
                    WHERE p.Id = @id";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                using SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int? userId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : null;

                    return new Post
                    {
                        Id = Convert.ToInt32(reader["PostId"]),
                        UserId = userId,
                        Content = reader["Content"].ToString(),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        User = userId.HasValue ? new User
                        {
                            Id = Convert.ToInt32(reader["UserId"]),
                            Username = reader["Username"].ToString(),
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            BirthDate = DateTime.Parse(reader["Birthday"].ToString())
                        } : null
                    };
                }
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
            return null;
        }
        public Post CreatePost(Post post)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "INSERT INTO Posts(UserId, Content, Date)\r\nVALUES (@UserId, @Content, @Date);\r\n; " + "SELECT LAST_INSERT_ROWID();";

                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@content", post.Content);
                command.Parameters.AddWithValue("@date", post.Date.ToString("yyyy-MM-dd"));

                return post;
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
        public bool Delete(int id)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM Posts WHERE Id = @id";
                using SqliteCommand command = new SqliteCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                int rows = command.ExecuteNonQuery();
                return rows > 0;
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

