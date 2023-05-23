using Auth.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.DAL;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
{
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Manager> Managers { get; set; } = null!;
    public DbSet<Cook> Cooks { get; set; } = null!;
    public DbSet<Courier> Couriers { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Cook)
            .WithOne(x => x.User)
            .HasForeignKey<Cook>().IsRequired();

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Manager)
            .WithOne(x => x.User)
            .HasForeignKey<Manager>().IsRequired();

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Courier)
            .WithOne(x => x.User)
            .HasForeignKey<Courier>().IsRequired();

        builder.Entity<ApplicationUser>()
            .HasOne(x => x.Customer)
            .WithOne(x => x.User)
            .HasForeignKey<Customer>().IsRequired();
    }
}