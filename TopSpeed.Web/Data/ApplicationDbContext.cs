using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Web.Models;

namespace TopSpeed.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }

        public DbSet<Brand> Brand { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<UserGarageItem> UserGarageItems { get; set; }
        public DbSet<Inquiry> Inquiries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Brand to Vehicles (one to many)
            builder.Entity<Vehicle>()
                .HasOne(v => v.Brand)
                .WithMany(b => b.Vehicles)
                .HasForeignKey(v => v.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserGarageItem to Vehicle
            builder.Entity<UserGarageItem>()
                .HasOne(g => g.Vehicle)
                .WithMany(v => v.GarageBookmarks)
                .HasForeignKey(g => g.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserGarageItem unique per user + vehicle
            builder.Entity<UserGarageItem>()
                .HasIndex(g => new { g.UserId, g.VehicleId })
                .IsUnique();
        }
    }
}
