using System.Reflection.Metadata.Ecma335;
using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace DrustveneMreze.Controllers
{
    [Route("api/user")]
    [ApiController]

    public class UserController : ControllerBase
    {

        private UserRepository userRepository = new UserRepository();

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
          

            List<User> users = GetAllUsers();
            return Ok(users);
        }

        private List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using SqliteConnection connection = new SqliteConnection("Data Source=Data/database.db");
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
            }
            

            return users;
        }
        

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            GroupRepository groupRepository = new GroupRepository();

            if (!UserRepository.Data.ContainsKey(id))
            {
                return NotFound();
            }
            return Ok(UserRepository.Data[id]);
        }

        [HttpPost]
        public ActionResult<User> Create([FromBody] User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.Name) || string.IsNullOrWhiteSpace(newUser.Surname) || newUser.BirthDate == DateTime.MinValue)
            {
                return BadRequest();
            }

            newUser.Id = SracunajNoviId(UserRepository.Data.Keys.ToList());
            UserRepository.Data[newUser.Id] = newUser;
            userRepository.Save();

            return Ok(newUser);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User uUser)
        {
            if (string.IsNullOrWhiteSpace(uUser.Username) || string.IsNullOrWhiteSpace(uUser.Name) || string.IsNullOrWhiteSpace(uUser.Surname) || uUser.BirthDate == DateTime.MinValue)
            {
                return BadRequest();
            }
            if (!UserRepository.Data.ContainsKey(id))
            {
                return NotFound();
            }

            User user = UserRepository.Data[id];
            user.Username = uUser.Username;
            user.Name = uUser.Name;
            user.Surname = uUser.Surname;
            user.BirthDate = uUser.BirthDate;
            userRepository.Save();

            return Ok(user);
        }


        private int SracunajNoviId(List<int> ids)
        {
            int maxId = 0;
            foreach (int id in ids)
            {
                if (id > maxId)
                {
                    maxId = id;
                }
            }
            return maxId + 1;
        }

    }
}
