using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;
public class ApplicationDbContext:DbContext{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){
    
    }

    public DbSet<Users> Users { get; set; }

    public DbSet<Categories> Categorys { get; set; }

    public DbSet<Tasks> Tasks { get; set; }
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
// {
//       modelBuilder.Entity<Tasks>()
//             .HasOne(t => t.User)
//             .WithMany(u => u.Tasks)
//             .HasForeignKey(t => t.UserId)
//             .OnDelete(DeleteBehavior.Restrict); // Restrict delete behavior
// }
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // User → Categories (Cascade Delete Allowed)
    modelBuilder.Entity<Users>()
        .HasMany(u => u.Categories)
        .WithOne(c => c.CreatedByUser)
        .HasForeignKey(c => c.CreatedByUserId)
        .OnDelete(DeleteBehavior.Cascade); // Safe because it does not create cycles

    // User → Tasks (No Action to prevent multiple cascade paths)
    modelBuilder.Entity<Users>()
        .HasMany(u => u.Tasks)
        .WithOne(t => t.User)
        .HasForeignKey(t => t.UserId)
        .OnDelete(DeleteBehavior.NoAction); // Prevents cascading delete conflict

    // Category → Tasks (Cascade Allowed)
    modelBuilder.Entity<Categories>()
        .HasMany(c => c.Tasks)
        .WithOne(t => t.Category)
        .HasForeignKey(t => t.CategoryId)
        .OnDelete(DeleteBehavior.Cascade); // Tasks get deleted if Category is deleted
}



    internal object Find(Guid id)
    {
        throw new NotImplementedException();
    }
}
