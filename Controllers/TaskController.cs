using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace authenticationandauthorization.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public TaskController(ApplicationDbContext dbContext)
        {
            _applicationDbContext = dbContext;
        }
         
      


        [Route("[controller]")]
        [Authorize]
        [HttpPost]
        public IActionResult CreateTasks(CreateTaskDto createTaskDto)
        {
            // Extract UserId from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not found in token");
            }

            Guid userId = Guid.Parse(userIdClaim.Value);

            // Check if the CategoryId exists in the Categories table
            if (createTaskDto.CategoryId.HasValue)
            {
                var category = _applicationDbContext.Categorys
                    .FirstOrDefault(c => c.Id == createTaskDto.CategoryId.Value);
                Console.WriteLine(createTaskDto.CategoryId);

                if (category == null)
                {
                    return BadRequest("Invalid CategoryId. The specified category does not exist.");
                }
            }

            // Create a new task
            var task = new Tasks
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                CategoryId = createTaskDto.CategoryId, // Optional if CategoryId is nullable
                UserId = userId
            };

            _applicationDbContext.Tasks.Add(task);
            _applicationDbContext.SaveChanges();

            return Ok("Task created successfully");
        }

        [Authorize]
        [HttpDelete("{Id:int}")]
        public IActionResult DeleteTask(int Id)
        {
            var task = _applicationDbContext.Tasks.Find(Id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            _applicationDbContext.Tasks.Remove(task);
            _applicationDbContext.SaveChanges();
            return Ok("Task deleted successfully.");
        }

        [Authorize]
        [HttpPut("{Id:int}")]
        public IActionResult UpdateTask(int Id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var task = _applicationDbContext.Tasks.Find(Id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Ensure the user owns the task
        

            // Update the task fields
            task.Title = updateTaskDto.Title ?? task.Title;
            task.Description = updateTaskDto.Description ?? task.Description;

            _applicationDbContext.Tasks.Update(task);
            _applicationDbContext.SaveChanges();

            return Ok("Task updated successfully.");
        }
    }
}
