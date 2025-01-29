using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using  YourNamespace.Models;
using Microsoft.AspNetCore.Authorization;
namespace authenticationandauthorization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _applicationDbContext = dbContext;
            _configuration = configuration;
        }
    [Authorize]
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var result = _applicationDbContext.Users.ToList();
        return Ok(result);
    }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            var name = loginUserDto.Name;
            var password = loginUserDto.Password;

            var user = _applicationDbContext.Users.FirstOrDefault(u => u.Name == name);
            if (user == null)
            {
                return BadRequest(new { message = "No user found" });
            }

            if (password == user.Password) // You should hash & verify passwords in production
            {
                var token = GenerateJwtToken(user.Name);
                return Ok(new { message = "User Login Successfully", token });
            }
            else
            {
                return BadRequest(new { message = "Password incorrect" });
            }
        }

        private string GenerateJwtToken(string name)
        {
            var secretKey = _configuration["JwtSettings:Secret"] ?? "thisismycustomsecretekeywhichiamusing";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha384);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "your.domain.com",
                audience: _configuration["JwtSettings:Audience"] ?? "your.domain.com",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] AddUserDto addUserDto)
        {
            var user = new Users
            {
                Name = addUserDto.Name,
                Email = addUserDto.Email,
                Password = addUserDto.Password, // Hash in production
                CustomRoless = addUserDto.CustomRoless
            };

            _applicationDbContext.Users.Add(user);
            _applicationDbContext.SaveChanges();

            return Ok(new { message = "User Registered Successfully" });
        }
    }
}
