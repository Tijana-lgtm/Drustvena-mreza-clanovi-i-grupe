using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private GroupRepository groupRepository = new GroupRepository();

        [HttpGet]
        public ActionResult<List<Group>> GetAll()
        {
            GroupRepository groupRepository = new GroupRepository();
            UserRepository userRepository = new UserRepository();

            List<Group> group = GroupRepository.Data.Values.ToList();
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
