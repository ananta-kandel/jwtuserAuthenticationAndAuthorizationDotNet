namespace YourNamespace.Models{

public class ShowCategoriesDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<TaskDto> Tasks { get; set; }
}
}