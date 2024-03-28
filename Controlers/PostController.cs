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

        [HttpGet("Posts")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts";
            return _dapper.LoadData<Post>(sql);

        }

        [HttpGet("PostSingle/{postId}")]
        public Post GetPostSingle(int postId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE PostId = @PostId";
            return _dapper.LoadDataSingle<Post>(sql, new {PostId = postId});
        }

        [HttpGet("GetPostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE UserId = @UserId";
            return _dapper.LoadData<Post>(sql, new {UserId = userId});
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post>  GetMyPosts()
        {
            string sql = "SELECT * FROM TutorialAppSchema.Posts WHERE UserId = @UserId";
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
        

        [HttpGet("PostsBySearch/{searchParam}")]
        public IEnumerable<Post>  PostsBySearch(string searchParam)
        {
            string sql = @"SELECT * 
                        FROM TutorialAppSchema.Posts
                        WHERE PostTitle LIKE @searchParam OR PostContent LIKE @searchParam";
            Console.WriteLine(sql);
            return _dapper.LoadData<Post>(sql, new { searchParam = '%' + searchParam + '%' });
        }

    }

}