using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace DrustveneMreze.Controllers
{
    [Route("api/groups/{groupId}/users/{userId}")]
    [ApiController]
    public class GroupMembersController : ControllerBase
    {
        private readonly UserDbRepository userDbRepository;
        private readonly GroupMembershipDbRepository GMDbRepository;
        private readonly GroupDbRepository groupDbRepository;

        public GroupMembersController(IConfiguration configuration)
        {
            GMDbRepository = new GroupMembershipDbRepository(configuration);
            groupDbRepository = new GroupDbRepository(configuration);
            userDbRepository = new UserDbRepository(configuration);
        }

        [HttpPut]
        public ActionResult<Group> Add(int groupId, int userId)
        {
            Group group = groupDbRepository.GetById(groupId);
            User user = userDbRepository.GetById(userId);
            if (group == null)
            {
                return NotFound($"Grupa {groupId} nije pronadjena.");
            }

            if (user == null)
            {
                return NotFound($"Korisnik {userId} nije pronadjena.");
            }
            try
            {
                bool success = GMDbRepository.AddUser(groupId, userId);
                if (!success)
                {
                    return Conflict($"Korisnik {userId} vec postoji u grupi.");
                }
                return Ok(group);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }

        //[HttpDelete("{groupId}")]
        //public ActionResult<User> Delete(int groupId, int userId)
        //{

        //    if (!GroupRepository.Data.ContainsKey(groupId))
        //    {
        //        return NotFound();
        //    }
        //    if (!UserRepository.Data.ContainsKey(userId))
        //    {
        //        return NotFound();
        //    }
        //    Group group = GroupRepository.Data[groupId];
        //    User user = UserRepository.Data[userId];

        //    if (!group.Users.Contains(user))
        //    {
        //        return BadRequest();
        //    }

        //    group.Users.Remove(user);
        //    user.Groups.Remove(group);
        //    groupRepository.Save();

        //    return Ok(user);
        //}
    }
}
