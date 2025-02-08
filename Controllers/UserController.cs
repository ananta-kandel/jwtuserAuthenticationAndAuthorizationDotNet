using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using  YourNamespace.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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


[Authorize(Roles = "User")]
[HttpGet("{id:Guid}")]  
public IActionResult GetUserById(Guid id)
{
    var user = _applicationDbContext.Users
        .Include(u => u.Categories)  
        .Include(u => u.Tasks)
        .FirstOrDefault(u => u.Id == id);  

    if (user == null)
    {
        return NotFound(new { message = "User not found" });
    }

    var userDto = new UserDto
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Tasks = user.Tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description
        }).ToList(),
        Categories = user.Categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList()
    };

    return Ok(userDto);
}

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAllUsers()
{
  
    var users = _applicationDbContext.Users
        .Include(u => u.Categories)  
        .Include(u => u.Tasks)
        .ToList();

    var userDtos = users.Select(u => new UserDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Tasks = u.Tasks.Select(c => new TaskDto{
            Id = c.Id,
            Title = c.Title,
            Description = c.Description
        }).ToList(),
        Categories = u.Categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        }).ToList() 
    }).ToList();

    return Ok(userDtos);
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

            if (password == user.Password) 
            {
                var token = GenerateJwtToken(user.Name,user.CustomRoless,user.Id);
                return Ok(new { message = "User Login Successfully", token });
            }
            else
            {
                return BadRequest(new { message = "Password incorrect" });
            }
        }

        private string GenerateJwtToken(string name, CustomRoless roless, Guid id)
        {
            var secretKey = _configuration["JwtSettings:Secret"] ?? "thisismycustomsecretekeywhichiamusing";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha384);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, name),
                new Claim(ClaimTypes.Role, roless.ToString()), // Add role claim
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
                Password = addUserDto.Password, 
                CustomRoless = addUserDto.CustomRoless
            };
            _applicationDbContext.Users.Add(user);
            _applicationDbContext.SaveChanges();
            return Ok(new { message = "User Registered Successfully" });
        }
    [HttpPut]
    [Route("{id:Guid}")]
    public IActionResult UpdateUser(Guid id,AddUserDto addUserDto){
        var result = _applicationDbContext.Users.Find(id);
        if(result == null){
            return Ok(new {message = "no user found"});
        }
        result.Name = addUserDto.Name;
        result.Email = addUserDto.Email;
        _applicationDbContext.SaveChanges();
        return Ok(new {message = "user updated sucessfully"});
    }
    }
}
