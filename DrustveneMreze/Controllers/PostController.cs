using DrustveneMreze.Domain;
using DrustveneMreze.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrustveneMreze.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbRepository postRepository;

        public PostController(IConfiguration configuration)
        {
            postRepository = new PostDbRepository(configuration);
        }

        [HttpGet]
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
    }
}