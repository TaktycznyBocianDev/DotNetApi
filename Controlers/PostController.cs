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


        // TutorialAppSchema.spPosts_Upsert
        // @UserId INT
        // , @PostTitle NVARCHAR(255)
        // , @PostContent NVARCHAR(MAX)
        // , @PostId INT = NULL

        [HttpPut("Post")]
        public IActionResult UpsertPost(Post postToAdd)
        {
            string sql = @"
                EXEC TutorialAppSchema.spPosts_Upsert
                    @UserId = @UserId,
                    @PostTitle = @PostTitle,
                    @PostContent = @PostContent";

            if (postToAdd.PostId > 0)
            {
                sql += ", @PostId = @PostId";
            }

            string? userId = this.User.FindFirst("userId")?.Value;
            if (_dapper.ExecuteSql(sql, new { UserId = userId, PostTitle = postToAdd.PostTitle, PostContent = postToAdd.PostContent, PostId = postToAdd.PostId }))
            {
                return Ok();
            }

            throw new Exception("Failed to create new post!");
        }


        [HttpDelete("Post/{postId}")]    
        public IActionResult DeletePost(int postId)
        { 
            string sql = @"EXEC TutorialAppSchema.spPosts_Delete @PostId = @PostId, @UserId = @UserId ";
            string? userId = this.User.FindFirst("userId")?.Value;
            if(_dapper.ExecuteSql(sql, new {PostId = postId.ToString(), UserId = userId})) return Ok();
            throw new Exception("Failed to delete post");
        }
        
    }

}