using Microsoft.EntityFrameworkCore;
using TaskManagementBackend.Controllers;

namespace TaskManagementBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.TaskItems)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
        }
    }
}