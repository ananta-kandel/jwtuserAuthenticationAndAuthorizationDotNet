namespace YourNamespace.Models{


public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    // Categories should be included in DTO
    public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
}

}