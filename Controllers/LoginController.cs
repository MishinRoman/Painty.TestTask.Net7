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
        private readonly IConfiguration _configuration;
        public LoginController(ApplicationDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _dbContext = context;
        }
       


        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(UserDTO request)
        {
            var user = _dbContext?.Users.FirstOrDefault(u => u.UserName == request.UserName);

           
            user.UserName = request.UserName;
            user.Password = request.Password;
            
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

                if (user == null && loginDTO.Password!=user.Password)
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
               (Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));

            var signinCredentials = new SigningCredentials
              (secretKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                   issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                   claims: claims,
                   expires: DateTime.Now.AddDays(10),
                   signingCredentials: signinCredentials
               );

            var jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);


            return jwt;
        }

        
        


    }
}
