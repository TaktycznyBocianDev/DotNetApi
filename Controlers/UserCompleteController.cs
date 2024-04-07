using System.Security;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;


namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration configuration)
    {
        _dapper = new DataContextDapper(configuration);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers/{userId}")] ///{isActive}
    public IEnumerable<UserComplete> GetUsers(int userId) //bool isActive
    {
        string sql = "EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if(userId != 0)
        {
            parameters += ", @UserId = @UserId";
        }
        
         sql += parameters.Substring(1);
        
        Console.WriteLine(sql);
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql, new { UserId = userId }); //, isActive = isActive
        return users;

    }

    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @" EXEC TutorialAppSchema.spUser_Upsert
                @FirstName = @FirstName, 
                @LastName = @LastName, 
                @Email = @Email, 
                @Gender = @Gender, 
                @JobTitlee = @JobTitlee
                @Department = @Department
                @Salary = @Salary
                @Active = @Active
                @UserId = @UserId";
        if(_dapper.ExecuteSql(sql, user))
        {
            return Ok();
        }

        throw new Exception("Failed to uppdate user");
        
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.Users WHERE UserId = @UserId";
        if(_dapper.ExecuteSql(sql, new { UserId = userId }))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user");
    }

    [HttpGet("GetUserSalaries")]
    public IEnumerable<UserSalary> GetUserSalaries()
    {
        string sql = "SELECT * FROM TutorialAppSchema.UserSalary";
        IEnumerable<UserSalary> salaries = _dapper.LoadData<UserSalary>(sql);
        return salaries;
    }

    
    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = @UserId";
        if(_dapper.ExecuteSql(sql, new { UserId = userId }))
        {
            return Ok();
        }

        throw new Exception("Failed to delete user salary");
    }

    [HttpGet("GetJobsInfo")]
    public IEnumerable<UserJobInfo> GetJobsInfo()
    {

        string sql = "SELECT * FROM TutorialAppSchema.UserJobInfo";
        IEnumerable<UserJobInfo> userJobsInfo = _dapper.LoadData<UserJobInfo>(sql);
        return userJobsInfo;

    }
    
    [HttpDelete("DeleteJobInfo/{userId}")]
    public IActionResult DeleteJobInfo(int userId)
    {
        string sql = "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = @userId";
         if(_dapper.ExecuteSql(sql, new {UserId = userId}))
        {
            return Ok();
        }

        throw new Exception("Failed to edit job info");
    }

}
