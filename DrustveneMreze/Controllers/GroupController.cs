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
