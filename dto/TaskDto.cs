namespace YourNamespace.Models{
    public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
}
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}
}