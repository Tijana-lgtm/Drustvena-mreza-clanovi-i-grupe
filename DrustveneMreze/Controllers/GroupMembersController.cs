using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMembersController : ControllerBase
    {
        private GroupRepository groupRepository = new GroupRepository();

        [HttpPut("{groupId}")]
        public ActionResult<User> Add(int groupId, int userId)
        {
            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                return NotFound();
            }

            if (!GroupRepository.Data.ContainsKey(userId))
            {
                return NotFound();
            }

            Group group = GroupRepository.Data[groupId];
            User user = UserRepository.Data[userId];

            if (!group.Users.Contains(user))
            {
                group.Users.Add(user);
            }
            return Ok(user);
        }

        [HttpDelete("{groupId}")]
        public ActionResult<User> Delete(int groupId, int userId)
        {

            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                return NotFound();
            }
            if (!UserRepository.Data.ContainsKey(userId))
            {
                return NotFound();
            }
            Group group = GroupRepository.Data[groupId];
            User user = UserRepository.Data[userId];

            if (!group.Users.Contains(user))
            {
                return BadRequest();
            }

            group.Users.Remove(user);
            user.Groups.Remove(group);
            groupRepository.Save();

            return Ok(user);
        }
    }
}
