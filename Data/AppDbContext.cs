using System.IO;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;

namespace NutriTrack.Data;

public class AppDbContext:DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<FoodLog> FoodLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string dbPath = Path.Combine(baseDir, "..", "..", "..", "nutritrack.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<FoodLog>()
            .HasOne(f => f.User)
            .WithMany(u => u.Logs)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<User>().HasData(
            new User { 
                Id = 1, 
                Username = "admin",
                Password = "admin",
                Name = "Адміністратор", 
                Age = 19, 
                Height = 196, 
                Weight = 93, 
                UserGender = Gender.Male, 
                Activity = ActivityLevel.Moderate 
            }
        );
    }
}