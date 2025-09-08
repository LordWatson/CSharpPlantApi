namespace PlantAPI.Models
{
    public class Plant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string CareInstructions { get; set; } = string.Empty;
        public string ExternalApiId { get; set; } = string.Empty;
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

