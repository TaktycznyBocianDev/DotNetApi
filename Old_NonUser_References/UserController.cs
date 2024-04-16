//Original User Controller - easy solution without stored procedures

// using System.Security;
// using DotnetAPI.Data;
// using DotnetAPI.Dtos;
// using DotnetAPI.Models;
// using Microsoft.AspNetCore.Mvc;


// namespace DotnetAPI.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class UserController : ControllerBase
// {
//     DataContextDapper _dapper;
//     public UserController(IConfiguration configuration)
//     {
//         _dapper = new DataContextDapper(configuration);
//     }

//     [HttpGet("TestConnection")]
//     public DateTime TestConnection()
//     {
//         return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
//     }

//     [HttpGet("GetUsers")]
//     public IEnumerable<User> GetUsers()
//     {
//         string sql = "SELECT * FROM TutorialAppSchema.Users";
//         IEnumerable<User> users = _dapper.LoadData<User>(sql);
//         return users;

//     }

//     [HttpGet("GetSingleUser/{userId}")]
//     public User GetSingleUser(int userId)
//     {
//         string sql = "SELECT * FROM TutorialAppSchema.Users WHERE UserId = @UserId";
//         return _dapper.LoadDataSingle<User>(sql, new { UserId = userId });
//     }

//     [HttpPut("EditUser")]
//     public IActionResult EditUser(User user)
//     {
//         string sql = @"
//         UPDATE TutorialAppSchema.Users 
//                 SET 
//                     [FirstName] = @FirstName, 
//                     [LastName] = @LastName, 
//                     [Email] = @Email, 
//                     [Gender] = @Gender, 
//                     [Active] = @Active
//                 WHERE UserId = @UserId";
//         if(_dapper.ExecuteSql(sql, user))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to uppdate user");
        
//     }

//     [HttpPost("AddUser")]
//     public IActionResult AddUser(UserDTO user)
//     {
//             string sql = @"
//              INSERT INTO TutorialAppSchema.Users(
//                 [FirstName], 
//                 [LastName], 
//                 [Email], 
//                 [Gender], 
//                 [Active])
//             VALUES (@FirstName, 
//                 @LastName, 
//                 @Email, 
//                 @Gender, 
//                 @Active)";
//         if(_dapper.ExecuteSql(sql, user))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to add user");
//     }

//     [HttpDelete("DeleteUser/{userId}")]
//     public IActionResult DeleteUser(int userId)
//     {
//         string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = @UserId";
//         if(_dapper.ExecuteSql(sql, new { UserId = userId }))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to delete user");
//     }

//     [HttpGet("GetUserSalaries")]
//     public IEnumerable<UserSalary> GetUserSalaries()
//     {
//         string sql = "SELECT * FROM TutorialAppSchema.UserSalary";
//         IEnumerable<UserSalary> salaries = _dapper.LoadData<UserSalary>(sql);
//         return salaries;
//     }

//     [HttpGet("GetUserSingleSalary/{userId}")]
//     public UserSalary GetUserSingleSalary(int userId)
//     {
//         string sql = "SELECT * FROM TutorialAppSchema.UserSalary WHERE UserID = @userId";
//         UserSalary salary = _dapper.LoadDataSingle<UserSalary>(sql, new {UserId = userId});
//         return salary;
//     }

//     [HttpPost("AddUserSalary/")]
//     public IActionResult AddUserSalary(UserSalary userSalary)
//     {
//             string sql = "INSERT INTO TutorialAppSchema.UserSalary([UserId],[Salary]) VALUES (@UserId, @Salary)";

//         if(_dapper.ExecuteSql(sql, userSalary))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to add user salary to database");
//     }

//     [HttpPut("EditUserSalary")]
//     public IActionResult EditUserSalary(UserSalary userSalary)
//     {
//         string sql = "UPDATE TutorialAppSchema.UserSalary SET [UserId] = @UserId, [Salary] = @Salary WHERE UserId = @userId";
//          if(_dapper.ExecuteSqlWithRowCount(sql, userSalary) > 0)
//         {
//             return Ok();
//         }

//         throw new Exception("No rows were updated");
//     }
    
//     [HttpDelete("DeleteUserSalary/{userId}")]
//     public IActionResult DeleteUserSalary(int userId)
//     {
//         string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = @UserId";
//         if(_dapper.ExecuteSql(sql, new { UserId = userId }))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to delete user salary");
//     }

//     [HttpGet("GetJobsInfo")]
//     public IEnumerable<UserJobInfo> GetJobsInfo()
//     {

//         string sql = "SELECT * FROM TutorialAppSchema.UserJobInfo";
//         IEnumerable<UserJobInfo> userJobsInfo = _dapper.LoadData<UserJobInfo>(sql);
//         return userJobsInfo;

//     }
    
//     [HttpGet("GetSingleJobInfo/{userId}")]
//     public UserJobInfo GetSingleJobInfo(int userId)
//     {
//         string sql = "SELECT * FROM TutorialAppSchema.UserJobInfo WHERE UserId = @userId";
//         UserJobInfo userJobsInfo = _dapper.LoadDataSingle<UserJobInfo>(sql, new {UserId = userId});
//         return userJobsInfo;
//     }

//     [HttpPost("AddJobInfo")]
//     public IActionResult AddJobInfo(UserJobInfo jobInfo)
//     {
//         string sql = "INSERT INTO TutorialAppSchema.UserJobInfo([UserId],[JobTitle],[Department]) VALUES (@UserId, @JobTitle, @Department)";
//         if(_dapper.ExecuteSql(sql, jobInfo))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to add job info");
//     }

//     [HttpPut("EditJobInfo")]
//     public IActionResult EditJobInfo(UserJobInfo jobInfo)
//     {
//         string sql = "UPDATE TutorialAppSchema.UserJobInfo SET UserId = @userId, JobTitle = @jobTitle, Department = @Department WHERE UserId = @userId";
//         if(_dapper.ExecuteSql(sql, jobInfo))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to edit job info");
//     }

//     [HttpDelete("DeleteJobInfo/{userId}")]
//     public IActionResult DeleteJobInfo(int userId)
//     {
//         string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = @userId";
//          if(_dapper.ExecuteSql(sql, new {UserId = userId}))
//         {
//             return Ok();
//         }

//         throw new Exception("Failed to edit job info");
//     }

// }
