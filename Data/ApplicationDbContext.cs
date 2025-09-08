/*
 * this is where .NET will setup the DB
 * as with a lot of .NET to Laravel comparisons, Laravel would split the logic found here across multiple files and directories
 *
 * migrations, models, relationships, constraints
 *
 * Laravel spreads its relationship types across the related models
 * User.php will have a HasMany to Plants
 * Plants.php with have a BelongsTo to Users
 *
 * .NET puts these relationship definitions into this 1 file
 */

using Microsoft.EntityFrameworkCore;
using PlantAPI.Models;

namespace PlantAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        /*
         * in Laravel we'd have a line:
         * protected $table = 'users';
         * in Models/User.php
         *
         * this defines the tables we use in our models
         */
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

/*
 * NOTES:
 *
 * query some users
 * .NET:
 * var users = await _context.Users.ToListAsync();
 *
 * Laravel:
 * $users = User::all();
 *
 *
 * include some relationships with the query
 *
 * .NET
 * var usersWithPlants = await _context.Users
    .Include(u => u.UserPlants)
    .ThenInclude(up => up.Plant)
    .ToListAsync();
 *
 * Laravel:
 * $usersWithPlants = User::with('userPlants.plant')->get();
 */