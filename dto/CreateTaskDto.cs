namespace YourNamespace.Models{


public class CreateTaskDto{

    public string Title { get; set; }
    public string Description { get; set; }

    public Guid UserId { get; set;}
   
    public int? CategoryId { get; set;}
}
}