using System.ComponentModel.DataAnnotations;

namespace PlantAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, EmailAddress] 
        public string Email { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        /*
         * Laravel equivalent to an eloquent relationship
         * here we just define that there will be a relationship
         * the type is defined in ApplicationDbContext.cs
         */
        public ICollection<UserPlant> UserPlants { get; set; } = new List<UserPlant>();
    }
}