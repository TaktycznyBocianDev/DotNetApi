using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controler]")]
    public class PostController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        
        public PostController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
        }

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts";
            return _dapper.LoadData<Post>(sql);

        }

        [HttpGet("PostSingle/{postId}")]
        public IEnumerable<Post> GetPostSingle(int postId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE PostId = @PostId";
            return _dapper.LoadData<Post>(sql, new {PostId = postId});
        }

        [HttpGet("GetPostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE UserId = @UserId";
            return _dapper.LoadData<Post>(sql, new {UserId = this.User.FindFirst("userId")?.Value});
        }

        [HttpPost("Post")]
        public IActionResult AddPost (PostToAddDTO postToAdd)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts 
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            VALUES (
                @UserId,
                @PostTitle,
                @PostContent,
                GETDATE(),
                GETDATE()
            )";

            if(_dapper.ExecuteSql(sql, new {UserId = this.User.FindFirst("userId")?.Value, 
                PostTitle = postToAdd.PostTitle, 
                PostContent = postToAdd.PostContent }))
            {

                return Ok();

            }

            throw new Exception("Failed to create new post!");
        }


    }

}