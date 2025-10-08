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
    public class GroupController : ControllerBase
    {
        private GroupRepository groupRepository = new GroupRepository();
        private UserRepository userRepository = new UserRepository();

        [HttpGet]
        public ActionResult<List<Group>> GetAll()
        {
            List<Group> groupsFromDb = GetAllFromDatabase();
            return Ok(groupsFromDb);
        }
        private List<Group> GetAllFromDatabase()
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

        [HttpPost]
        public ActionResult<Group> Create([FromBody] Group newGroup)
        {
            if (string.IsNullOrWhiteSpace(newGroup.GroupName) || newGroup.Incorporation == DateTime.MinValue)
            {
                return BadRequest();
            }

            newGroup.Id = GetNewId(GroupRepository.Data.Keys.ToList());
            GroupRepository.Data[newGroup.Id] = newGroup;
            groupRepository.Save();

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
            groupRepository.Save();

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
