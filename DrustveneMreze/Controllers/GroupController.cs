using System.Security.Cryptography.Xml;
using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;


namespace DrustveneMreze.Controllers
{
    [Route("api/groups")]
    [ApiController]

    public class GroupDbRepository
    {
        public Group GetById(int id)
        {
            Group group = null;

            try
            {
                SqliteConnection connection = new SqliteConnection("Data Source=Data/database.db");
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
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili SQL upitu: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Pogresna operacija nad konekcijom ili komandama: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
            }

            return group;
        }
        public List<Group> GetAll()
        {
            List<Group> groups = new List<Group>();

            try
            {
                SqliteConnection connection = new SqliteConnection("Data Source=Data/database.db");
                connection.Open();

                string query = "SELECT Id, Name, CreationDate FROM Groups";

                using (SqliteCommand command = new SqliteCommand(query, connection))
                {
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
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"Greska pri konekciji ili SQL upitu: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Pogresna operacija nad konekcijom ili komandama: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Neocekivana greska: {ex.Message}");
            }

            return groups;
        }
    }
    public class GroupController : ControllerBase
    {
        private readonly GroupDbRepository groupRepository;
        private UserRepository userRepository = new UserRepository();

        public GroupController()
        {
            groupRepository = new GroupDbRepository();
        }
        [HttpGet]
        public ActionResult<List<Group>> GetAll()
        {
            List<Group> groupsFromDb = groupRepository.GetAll();
            return Ok(groupsFromDb);
        }

        [HttpGet("{id}")]
        public ActionResult<Group> GetById(int id)
        {
            Group group = groupRepository.GetById(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }


        [HttpPost]
        public ActionResult<Group> Create([FromBody] Group newGroup)
        {
            if (string.IsNullOrWhiteSpace(newGroup.GroupName) || newGroup.Incorporation == DateTime.MinValue)
            {
                return BadRequest();
            }

            newGroup.Id = GetNewId(GroupRepository.Data.Keys.ToList());
            GroupRepository.Data[newGroup.Id] = newGroup;

            return Ok(newGroup);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            if (!GroupRepository.Data.ContainsKey(id))
            {
                return NotFound();
            }

            GroupRepository.Data.Remove(id);

            return NoContent();
        }

        private int GetNewId(List<int> ids)
        {
            int maxId = 0;
            foreach(int id in ids)
            {
                if(id > maxId)
                {
                    maxId = id;
                }
            }
            return maxId + 1;
        }
    }
}
