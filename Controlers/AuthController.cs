using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    public class AuthController : ControllerBase
    {
        
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _dapper = new DataContextDapper(configuration);
            _configuration = configuration;
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

                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth(
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES (
                            @Email, 
                            @PasswordHash, 
                            @PasswordSalt
                        )";

                    if (_dapper.ExecuteSql(sqlAddAuth, new
                    {
                        Email = emailForRegistration,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    }))
                    {
                         string sqlAddUser = @"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName], 
                                [LastName], 
                                [Email], 
                                [Gender])
                            VALUES (@FirstName, 
                                @LastName, 
                                @Email, 
                                @Gender)";

                        if(_dapper.ExecuteSql(sqlAddUser, new {FirstName = userForRegistration.FirstName,
                                LastName = userForRegistration.LastName,
                                Email = userForRegistration.Email,
                                Gender = userForRegistration.Gender}))
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

            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            for(int i = 0; i < passwordHash.Length; i++)
            {
                if( passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect password");
                }
            }

            string sqlUserId = "SELECT UserId FROM TutorialAppSchema.Users WHERE Email = @Email";
            int userId = _dapper.LoadDataSingle<int>(sqlUserId, new {Email = userForLogin.Email});

            return Ok(new Dictionary<string, string> {
                {"token", CreateToken(userId)}
            });
        }

       // [HttpGet]

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltWithString =
                      _configuration.GetSection("AppSettings.PasswordKey").Value +
                          Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltWithString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
        }

        private string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("userId", userId.ToString())
            };

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:TokenKey").Value));

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }

}
