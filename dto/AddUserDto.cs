namespace YourNamespace.Models
{
    public class AddUserDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public CustomRoless CustomRoless { get; set; } = CustomRoless.User;
    }
}
