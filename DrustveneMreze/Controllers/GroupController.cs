using System.Security.Cryptography.Xml;
using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;


namespace DrustveneMreze.Controllers
{
    [Route("api/groups")]
    [ApiController]

    
    public class GroupController : ControllerBase
    {
        private readonly GroupDbRepository groupRepository;
        private UserRepository userRepository = new UserRepository();

        public GroupController(IConfiguration configuration)
        {
            groupRepository = new GroupDbRepository (configuration);
        }

        [HttpGet]
        public ActionResult<List<Group>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                List<Group> groupsFromDb = groupRepository.GetAll(page, pageSize);
                int totalCount = groupRepository.CountAll();
                return Ok(new
                {
                    Data = groupsFromDb,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                return Problem("An error occurred while fetching the group.");
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Group> GetById(int id)
        {
            try
            {
                Group group = groupRepository.GetById(id);
                if (group == null)
                {
                    return NotFound();
                }
                return Ok(group);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Greska:" + ex.ToString());
                return Problem(detail: ex.Message, title: "Database error");
            }

        }


        [HttpPost]
        public ActionResult<Group> Create([FromBody] Group newGroup)
        {
            if (string.IsNullOrWhiteSpace(newGroup.GroupName) || newGroup.Incorporation == DateTime.MinValue)
            {
                return BadRequest();
            }
            try
            {
                int newId = groupRepository.Create(newGroup);
                if (newId == -1)
                {
                    return StatusCode(500, "Greska pri kreiranju grupe.");
                }

                newGroup.Id = newId;
                return Ok(newGroup);
            }
            catch
            {
                return Problem("An error occurred while creating the group.");
            }

        }

        [HttpPut("{id}")]
        public ActionResult Update(int id, [FromBody] Group updatedGroup)
        {
            try
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
            catch
            {
                return Problem("An error occurred while updating the group.");
            }

        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            try
            {
                bool deleted = groupRepository.Delete(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch
            {
                return Problem("An error occurred while deleting the group.");
            }
        }
    }
}
