using Microsoft.EntityFrameworkCore;
using PlantAPI.Models;

namespace PlantAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Plant> Plants { get; set; }
        public DbSet<UserPlant> UserPlants { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // this is where the type of relationship is defined (Laravel puts all this into the Models)
            modelBuilder.Entity<UserPlant>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPlants)
                .HasForeignKey(up => up.UserId);
            
            modelBuilder.Entity<UserPlant>()
                .HasOne(up => up.Plant)
                .WithMany(p => p.UserPlants)
                .HasForeignKey(up => up.PlantId);
            
            // make sure the user email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }    
}