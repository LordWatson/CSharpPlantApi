namespace PlantAPI.Models
{
    public class UserPlant
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlantId { get; set; }
        
        // these fields are custom per users plant
        public string PlantNickname { get; set; } = string.Empty;
        public string HealthStatus { get; set; } = "Good";
        public DateTime? LastWatered { get; set; }
        public DateTime? LastFertilized { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        /*
         * Laravel equivalent to an eloquent relationship
         * here we just define that there will be a relationship
         * the type is defined in ApplicationDbContext.cs
         */
        public User User { get; set; }
        public Plant Plant { get; set; }
    }
}