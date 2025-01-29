using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;
public class ApplicationDbContext:DbContext{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){
    
    }

    public DbSet<Users> Users { get; set; }

    internal object Find(Guid id)
    {
        throw new NotImplementedException();
    }
}
