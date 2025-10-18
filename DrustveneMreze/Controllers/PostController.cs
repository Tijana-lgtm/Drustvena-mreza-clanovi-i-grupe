using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbRepository postRepository;
        private readonly UserDbRepository userRepository;


        public PostController(IConfiguration configuration)
        {
            postRepository = new PostDbRepository(configuration);
            userRepository = new UserDbRepository(configuration);

        }

        [HttpGet ("posts")]
        public ActionResult<List<Post>> GetAll()
        {
            try
            {
                List<Post> posts = postRepository.GetAll();
                return Ok(posts);
            }
            catch
            {
                return Problem("An error occurred while fetching posts.");
            }
        }

        [HttpPost("users/{userId}/posts")]
        public ActionResult<Post> CreatePost(int userId, [FromBody] Post newPost)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newPost.Content))
                {
                    return BadRequest("Post field can't be empty.");
                }

                User user = userRepository.GetById(userId);
                if (user == null)
                {
                    return NotFound( "User" + userId + "does not exist.");
                }

                newPost.UserId=user.Id;
                newPost.Date=DateTime.Now;

                Post createdPost = postRepository.CreatePost(newPost);

                if (createdPost == null)
                {
                    return BadRequest("Creting post error");
                }
                return Ok(createdPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Kreiranje korisnika nije uspesno, greska: " + ex.Message);
            }
        }

        [HttpDelete("posts/{postId}")]
        public ActionResult Delete(int postId)
        {
            try
            {
                Post deletePost = postRepository.GetById(postId);
                if (deletePost == null)
                {
                    return NotFound("Objava ne postoji.");
                }
                bool successfullDel = postRepository.Delete(postId);
                if (!successfullDel)
                {
                    return StatusCode(500, "Greska pri brisanju objave.");
                }
                return Ok("Objava uspesno obrisana");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Objava nije obrisana, greska: " + ex.Message);
            }
        }
    }
}