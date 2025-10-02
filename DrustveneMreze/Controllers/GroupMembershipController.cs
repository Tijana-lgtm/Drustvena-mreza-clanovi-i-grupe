using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMembershipController : ControllerBase
    {
        private GroupRepository groupRepository = new GroupRepository();

        [HttpGet("{groupId}/users")]
        public ActionResult<List<User>> GetUsersByGroupId(int groupId)
        {
            if (!GroupRepository.Data.ContainsKey(groupId))
            {
                return NotFound($"Group with id {groupId} not found.");
            }

            Group group = GroupRepository.Data[groupId];
            return Ok(group.Users);
        }
    }
}
