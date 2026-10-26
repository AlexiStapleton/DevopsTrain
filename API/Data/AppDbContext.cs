using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public AppDbContext() {}
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("devops_training");
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(p => p.Id);
        });
    }
    
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}