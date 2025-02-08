namespace YourNamespace.Models;
public class Tasks{

    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Guid? UserId { get; set;}
    public Users? User { get; set;}

    // Foreign key for Category
    public int? CategoryId { get; set; }
    public Categories? Category { get; set; } 
}