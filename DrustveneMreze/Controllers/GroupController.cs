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

            int newId = groupRepository.Create(newGroup);
            if (newId == -1)
            {
                return StatusCode(500, "Greska pri kreiranju grupe.");
            }

            newGroup.Id = newId;
            return Ok(newGroup);
        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Group updatedGroup)
        {
            if (id != updatedGroup.Id)
            {
                return BadRequest("ID u putanji i u telu zahteva se ne poklapaju.");
            }

            if (string.IsNullOrWhiteSpace(updatedGroup.GroupName) || updatedGroup.Incorporation == DateTime.MinValue)
            {
                return BadRequest();
            }

            bool updated = groupRepository.Update(updatedGroup);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            bool deleted = groupRepository.Delete(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}
