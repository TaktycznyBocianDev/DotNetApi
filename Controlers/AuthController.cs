using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly DataContextDapper _dapper;

        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
            _authHelper = new AuthHelper(configuration);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration)
        {

            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string emailForRegistration = userForRegistration.Email;
                string sqlUserExist = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = @Email";
                Console.WriteLine(sqlUserExist);

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlUserExist, new { Email = userForRegistration.Email });
                if (existingUsers.Count() == 0)
                {

                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @" TutorialAppSchema.spRegistration_Upsert
                            @Email, 
                            @PasswordHash, 
                            @PasswordSalt
                        ";

                    if (_dapper.ExecuteSql(sqlAddAuth, new
                    {
                        Email = emailForRegistration,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }))
                    {                        
                        string sqlAddUser = @" EXEC TutorialAppSchema.spUser_Upsert
                                @FirstName = @FirstName, 
                                @LastName = @LastName, 
                                @Email = @Email, 
                                @Gender = @Gender, 
                                @JobTitle = @JobTitle,
                                @Department = @Department,
                                @Salary = @Salary";

                        if (_dapper.ExecuteSql(sqlAddUser, new
                        {
                            FirstName = userForRegistration.FirstName,
                            LastName = userForRegistration.LastName,
                            Email = userForRegistration.Email,
                            Gender = userForRegistration.Gender,
                            JobTitle = userForRegistration.JobTitle,
                            Department = userForRegistration.Department,
                            Salary = userForRegistration.Salary
                        }))
                        {

                            return Ok();

                        }

                        throw new Exception("Failed to add user!");
                    }

                    throw new Exception("Failed to register user!");
                }

                throw new Exception("User with this email exists!");

            }

            throw new Exception("Password do not match!");

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {

            string sqlForHashAndSalt = @"SELECT [PasswordHash], [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = @email";

            UserForLoginConfirmationDTO userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDTO>(sqlForHashAndSalt, new { Email = userForLogin.Email });

            byte[] passwordHash =  _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }

            string sqlUserId = "SELECT UserId FROM TutorialAppSchema.Users WHERE Email = @Email";
            int userId = _dapper.LoadDataSingle<int>(sqlUserId, new { Email = userForLogin.Email });

            return Ok(new Dictionary<string, string> {
                {"token",  _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = "SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = @UserId";

            int userId = _dapper.LoadDataSingle<int>(userIdSql, new { UserId = User.FindFirst("userId")?.Value });

            return  _authHelper.CreateToken(userId);
        }

    }

}
