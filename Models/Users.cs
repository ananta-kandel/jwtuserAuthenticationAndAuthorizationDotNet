namespace YourNamespace.Models
{
    public class Users
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public CustomRoless CustomRoless { get; set; } = CustomRoless.User; 
    }
}
