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

        private readonly UserDbRepository userRepository;

        public UserController()
        {
            userRepository = new UserDbRepository();
        }

        [HttpGet]
        public ActionResult<List<User>> GetAll()
        {       
            List<User> users = userRepository.GetAll();
            return Ok(users);
        }

        
        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {


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

            var createdUser = userRepository.CreateUser(newUser);
            if (createdUser == null)
            {
                return StatusCode(500, "Greska pri kreiranju korisnika.");
            }
            return Ok(createdUser);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User uUser)
        {
            if (string.IsNullOrWhiteSpace(uUser.Username) || string.IsNullOrWhiteSpace(uUser.Name) || string.IsNullOrWhiteSpace(uUser.Surname) || uUser.BirthDate == DateTime.MinValue)
            {
                return BadRequest();
            }
            var existingUser = userRepository.GetById(id);
            if (existingUser == null)
            {
                return NotFound("Korisnik nije pronadjen.");
            }

            bool success = userRepository.UpdateUser(id, uUser);
            if (!success)
            {
                return StatusCode(500, "Greska pri azuriranju korisnika.");
            }
            uUser.Id = id;
            return Ok(uUser);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var deleteUser = userRepository.GetById(id);
            if(deleteUser == null)
            {
                return NotFound("Korisnik ne postoji.");
            }

            bool sucessfullDel = userRepository.Delete(id);
            if (!sucessfullDel)
            {
                return StatusCode(500, "Greska pri brisanju korisnika.");
            }
            return Ok("Korisnik uspesno obrisan");
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
