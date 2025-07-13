using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Bazis.Models;
using Microsoft.AspNetCore.Identity;


namespace Bazis.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bazis.Models.News> News { get; set; } = default!;
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<ServiceCenter> ServiceCenters { get; set; }
    public DbSet<ServiceType>   ServiceTypes   { get; set; }
    public DbSet<Lift>          Lifts          { get; set; }
    public DbSet<Slot>          Slots          { get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Явно настраиваем связь «Order → ApplicationUser»
        builder.Entity<Order>()
        .HasOne<IdentityUser>(o => o.User)
       .WithMany()                    // без коллекции на стороне User
       .HasForeignKey(o => o.UserId)
       .OnDelete(DeleteBehavior.Cascade);


        // Явно настраиваем связь Car → ApplicationUser»
        builder.Entity<Car>()
       .HasOne<IdentityUser>(o => o.User)
       .WithMany()                    // без коллекции на стороне User
       .HasForeignKey(o => o.UserId)
       .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<Car>()
       .HasMany(e => e.Orders)
       .WithOne(e => e.Car)
       .HasForeignKey(e => e.CarId)
       .IsRequired(false);


         builder.Entity<ServiceCenter>()
          .HasMany(sc => sc.Lifts)
          .WithOne(l => l.ServiceCenter)
          .HasForeignKey(l => l.ServiceCenterId);

        builder.Entity<Lift>()
          .HasMany<Slot>()
          .WithOne(s => s.Lift)
          .HasForeignKey(s => s.LiftId);

        builder.Entity<Slot>()
          .HasIndex(s => new { s.ServiceCenterId, s.Date, s.Time, s.IsBooked });
        builder.Entity<Slot>()
          .HasIndex(s => new { s.ServiceCenterId, s.LiftId, s.Date, s.Time })
          .IsUnique();

    }

public DbSet<Bazis.Models.Car> Car { get; set; } = default!;

public DbSet<Bazis.Models.CatalogCar> CatalogCar { get; set; } = default!;


}
