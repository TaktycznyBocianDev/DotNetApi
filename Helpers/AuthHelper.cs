using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{

    public class AuthHelper
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _configuration;

        public AuthHelper(IConfiguration config)
        {
            _configuration = config;
            _dapper = new DataContextDapper(config);
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

        public string CreateToken(int userId)
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

         public bool SetPassword(UserForLoginDTO userForPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash =  GetPasswordHash(userForPassword.Password, passwordSalt);

            string sqlAddAuth = @" TutorialAppSchema.spRegistration_Upsert
                    @Email, 
                    @PasswordHash, 
                    @PasswordSalt
                ";

            return _dapper.ExecuteSql(sqlAddAuth, new  {Email = userForPassword.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt});
            
        }
    }

}

