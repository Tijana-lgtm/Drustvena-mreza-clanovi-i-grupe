using Microsoft.Data.Sqlite;

namespace DrustveneMreze.Repositories
{
    public class GroupMembershipDbRepository
    {
        private readonly string connectionStr;

        public GroupMembershipDbRepository(IConfiguration configuration)
        {
            connectionStr = configuration["ConnectionString:SQLiteConnection"];
        }



        public bool AddUser(int groupId, int userId)
        {
            try
            {
                using SqliteConnection connection = new SqliteConnection(connectionStr);
                connection.Open();

                var checkCommand = connection.CreateCommand();
                checkCommand.CommandText = @"SELECT COUNT(*) FROM GroupMemberships WHERE UserId = @UserId AND GroupId = @GroupId";

                checkCommand.Parameters.AddWithValue("@GroupId", groupId);
                checkCommand.Parameters.AddWithValue("@UserId", userId);

                long i = (long)checkCommand.ExecuteScalar();
                if(i > 0)
                {
                    return false;
                }

                string query = "INSERT INTO GroupMemberships (GroupId, UserId) VALUES (@GroupId, @UserId);";
                using SqliteCommand command = new SqliteCommand(query, connection);

                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@UserId", userId);

                int affectedRows = command.ExecuteNonQuery();
                return affectedRows > 0;
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
