using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {

        private readonly DataContextDapper _dapper;
        
        public PostController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchValue}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchValue = "none")
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            if (postId != 0) parameters += ", @PostId = @PostId";
            if (userId != 0) parameters += ", @UserId = @UserId";
            if (searchValue.ToLower() != "none") parameters += ", @SearchValue = @SearchValue";


            if(parameters.StartsWith(",")) sql += parameters.Substring(1);

            return _dapper.LoadData<Post>(sql, new {@PostId = postId, @UserId = userId, @SearchValue = searchValue});

        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post>  GetMyPosts()
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Get @UserId = @UserId";
            return _dapper.LoadData<Post>(sql, new {UserId = this.User.FindFirst("userId")?.Value});
        }

        [HttpPost("AddPost")]
        public IActionResult AddPost (PostToAddDTO postToAdd)
        {
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts 
                (UserId,
                PostTitle,
                PostContent,
                PostCreated,
                PostUpdated)
            VALUES (
                @UserId,
                @PostTitle,
                @PostContent,
                GETDATE(),
                GETDATE()
            )";

            string? userId = this.User.FindFirst("userId")?.Value;

            if(_dapper.ExecuteSql(sql, new {UserId = userId, 
                PostTitle = postToAdd.PostTitle, 
                PostContent = postToAdd.PostContent }))
            {

                return Ok();

            }

            throw new Exception("Failed to create new post!");
        }

        [HttpPut("EditPost")]
        public IActionResult EditPost (PostToEditDTO postToEdit)
        {
            string sql = @"UPDATE TutorialAppSchema.Posts SET [PostContent] = @PostContent, [PostTitle] = @PostTitle, [PostUpdated] = GETDATE() WHERE PostId = @PostId AND UserId = @UserId";

            if(_dapper.ExecuteSql(sql, new {PostContent = postToEdit.PostContent, 
            PostTitle = postToEdit.PostTitle, 
            PostId = postToEdit.PostId, 
            UserId = this.User.FindFirst("userId")?.Value})) 
            {

                return Ok();

            }

            throw new Exception("Failed to update post!");
        }

        [HttpDelete("DeletePost/{postId}")]    
        public IActionResult DeletePost(int postId)
        {
            string sql = @"DELETE FROM TutorialAppSchema.Posts WHERE PostId = @PostId";
            if(_dapper.ExecuteSql(sql, new {PostId = postId})) return Ok();
            throw new Exception("Failed to delete post");
        }
        
    }

}