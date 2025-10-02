using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private UserRepository userRepository = new UserRepository();

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {
            GroupRepository groupRepository = new GroupRepository();
            UserRepository userRepository = new UserRepository();

            List<User> users = UserRepository.Data.Values.ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            GroupRepository groupRepository = new GroupRepository();
            UserRepository userRepository = new UserRepository();
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
