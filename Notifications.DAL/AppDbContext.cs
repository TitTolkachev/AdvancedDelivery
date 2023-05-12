using Microsoft.EntityFrameworkCore;
using Notifications.DAL.Entities;

namespace Notifications.DAL;

public sealed class AppDbContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>().HasKey(x => x.Id);
    }
}