using System.Text.Json.Serialization;
namespace YourNamespace.Models
{
    public class Users
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public CustomRoless CustomRoless { get; set; } = CustomRoless.User; 

        public ICollection<Tasks> Tasks { get; } = new List<Tasks>();

  
        public ICollection<Categories> Categories { get; } = new List<Categories>();
        
    }
}
