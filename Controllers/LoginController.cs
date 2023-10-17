using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using Painty.TestTask.Net7.Data.Models;
using Painty.TestTask.Net7.Data.Models.DTO;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Painty.TestTask.Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public LoginController(ApplicationDbContext context)
        {
            _dbContext = context;
        }
       


        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(UserDTO request)
        {
            var user = _dbContext?.Users.FirstOrDefault(u => u.UserName == request.UserName);

            GretePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.UserName = request.UserName;
            user.Password = request.Password;
            user.PasswordHash = passwordHash.ToString();//ToString дает значение  "Byte[]"!!!
            user.SecurityStamp = passwordSalt.ToString();
            user.Name = user.UserName;
            user.PhoneNumber = "123";
            user.Email = user.Name;
            user.EmailConfirmed = true;
            user.PhotoUrl = "/";


            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(user);
        }


        [HttpPost, Route("login")]
        public IActionResult Login(UserDTO loginDTO)
        {

            try
            {

                if (string.IsNullOrEmpty(loginDTO.UserName) ||
                string.IsNullOrEmpty(loginDTO.Password))
                    return BadRequest("Username and/or Password not specified");

                var user = _dbContext?.Users.FirstOrDefault(u => u.UserName == loginDTO.UserName);

                if (user == null && !VerifyPasswordHash(loginDTO.Password, Encoding.UTF8.GetBytes(user.Password), Encoding.UTF8.GetBytes(user.SecurityStamp)))
                { return BadRequest("Error authorisation"); }


                string token = CreateToken(user);
                if (!token.IsNullOrEmpty())
                {
                    return Ok(token);

                }
                return BadRequest
                 ("An error occurred in generating the token");

            }


            catch(Exception ex) 
            {
                return BadRequest
                (ex.Message);

            }

        }

        private string CreateToken(AppUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                    new Claim(ClaimTypes.Name, user.UserName)

            };
            SymmetricSecurityKey? secretKey = new SymmetricSecurityKey
               (Encoding.UTF8.GetBytes("thisisasecretkey@123456989101112"));

            var signinCredentials = new SigningCredentials
              (secretKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                   
                   claims: claims,
                   expires: DateTime.Now.AddDays(10),
                   signingCredentials: signinCredentials
               );

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


            return jwt;
        }

        private bool VerifyPasswordHash(string passwor, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwor));
                return computerHash.SequenceEqual(passwordHash);
            }
        }
        private void GretePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }


    }
}
