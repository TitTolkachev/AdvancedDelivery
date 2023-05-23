using Backend.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAL;

public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Rating> Ratings { get; set; } = null!;
    public DbSet<Dish> Dishes { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Cart> Carts { get; set; } = null!;
    public DbSet<Token> Tokens { get; set; } = null!;
    public DbSet<Restaurant> Restaurants { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>().HasKey(x => x.Id);
        modelBuilder.Entity<Rating>()
            .HasIndex(x => new { x.DishId, x.UserId })
            .IsUnique();

        modelBuilder.Entity<Dish>().HasKey(x => x.Id);

        modelBuilder.Entity<Order>().HasKey(x => x.Id);

        modelBuilder.Entity<User>().HasKey(x => x.Id);

        modelBuilder.Entity<Cart>().HasKey(x => x.Id);
        modelBuilder.Entity<Cart>()
            .HasIndex(x => new { x.DishId, x.UserId, x.OrderId })
            .IsUnique();
        modelBuilder.Entity<Cart>()
            .Property(x => x.OrderId)
            .IsRequired(false);

        modelBuilder.Entity<Token>().HasKey(x => x.InvalidToken);

        modelBuilder.Entity<Restaurant>().HasKey(x => x.Id);
        modelBuilder.Entity<Restaurant>().HasIndex(x => x.Name).IsUnique();
    }
}