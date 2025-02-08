using System.Text.Json.Serialization;
namespace YourNamespace.Models;
public class Categories{
    public int Id { get; set; }
    public string Name { get; set; }

    public Guid CreatedByUserId { get; set; }
    public Users CreatedByUser { get; set; }
    
    [JsonIgnore]
    public ICollection<Tasks> Tasks { get; set; } = new List<YourNamespace.Models.Tasks>();
}