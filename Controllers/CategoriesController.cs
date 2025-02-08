using Microsoft.AspNetCore.Mvc;
using  YourNamespace.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
namespace authenticationandauthorization.Controllers

{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public  CategoriesController(ApplicationDbContext dbContext)
        {
            _applicationDbContext = dbContext;
        }

[HttpGet("allCategories")]
public IActionResult GetAllCategories()
{
    // Fetch categories and their associated tasks from the database
    var categories = _applicationDbContext.Categorys
        .Include(c => c.Tasks) // Include the tasks related to each category
        .ToList(); // Execute the query

    // Transform the data into DTOs
    var categoryDtos = categories.Select(c => new ShowCategoriesDto
    {
        Id = c.Id,
        Name = c.Name,
        Tasks = c.Tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description
        }).ToList()
    }).ToList();

    // Return the result as JSON
    return Ok(categoryDtos);
}

        [Authorize]
        [HttpDelete]
        // [HttpDelete("allCategories")]
        [Route("{Id:int}")]
        public IActionResult DeleteCategories(int Id){
            var result = _applicationDbContext.Categorys.Find(Id);
            _applicationDbContext.Categorys.Remove(result);
            _applicationDbContext.SaveChanges();
            return Ok();  
        }
        [Authorize]
        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateCategories(int id, CategorieDto categoriesDto ){
            var result = _applicationDbContext.Categorys.Find(id);
            if(result == null){
                return Ok("no data found");
            }
            result.Name = categoriesDto.Name;
           _applicationDbContext.SaveChanges();
           return Ok("Data updated sucessfully");
        }
        [Authorize]
        [HttpPost("create")]
    public IActionResult CreateCategories(CategorieDto categoriesDto)
    {
        // Extract UserId from JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized("User not found in token");
        }
        Guid userId = Guid.Parse(userIdClaim.Value); 
        var categories = new Categories
        {
            Name = categoriesDto.Name,
            CreatedByUserId = userId 
        };
        _applicationDbContext.Categorys.Add(categories);
        _applicationDbContext.SaveChanges();
        return Ok("Categories Created Successfully");
    }    
}
}

