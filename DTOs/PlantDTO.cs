/*
 * these are similar to FormRequests in Laravel 
 * this file also handles the response formatting - @see AuthResponse
 * 
 * .NET will put multiple "Request" classes into one file
 * where Laravel we'd use a file per-request type - @see LoginRequest and RegisterRequest are in the same file
 */
namespace PlantAPI.DTOs
{
    public class SearchPlantRequest
    {
        public string PlantName { get; set; } = string.Empty;
    }

    public class AddPlantRequest
    {
        public string Nickname { get; set; } = string.Empty;
    }

    public class UpdateUserPlantRequest
    {
        public string? PlantNickname { get; set; }
        public string? HealthStatus { get; set; }
        public DateTime? LastWatered { get; set; }
        public DateTime? LastFertilized { get; set; }
        public string? Notes { get; set; }
    }
}