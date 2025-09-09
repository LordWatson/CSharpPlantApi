/*
 * this is just what you'd expect from a Service class in Laravel
 * it implements the Interface as seen by the syntax:
 * PlantService : IPlantService
 *
 * aside from that, all we're doing is creating the functions we defined in the Interface
 * and writing the business logic in there, to keep it out of the Controller
 * exactly how we do in Laravel
 */
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PlantAPI.Data;
using PlantAPI.Models;

namespace PlantAPI.Services
{
    public class PlantService : IPlantService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlantService> _logger;

        public PlantService(ApplicationDbContext context, HttpClient httpClient, ILogger<PlantService> logger)
        {
            _context = context;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Plant?> GetPlantFromExternalApiAsync(string plantName)
        {
            try
            {
                // perenual api to query plants 
                var apiUrl = $"https://perenual.com/api/species-list?key=THIS_IS_WHERE_MY_API_KEY_WILL_GO&q={Uri.EscapeDataString(plantName)}";
                
                // mockup a response while I don't have my API setup properly
                var response = await _httpClient.GetAsync(apiUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    
                    // parse JSON response according
                    // This is a simplified example
                    var plant = new Plant
                    {
                        Name = plantName,
                        ScientificName = $"{plantName} scientificus", // from API
                        Description = $"A beautiful {plantName} plant", // from API
                        ImageUrl = "https://example.com/plant.jpg", // from API
                        CareInstructions = "Water regularly, provide adequate sunlight",
                        ExternalApiId = "external-123"
                    };
                    
                    return plant;
                }
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching plant data from external API");
            }

            // fake data to return while we don't have the API
            return new Plant
            {
                Name = plantName,
                ScientificName = $"{plantName} mock",
                Description = $"Mock data for {plantName}",
                ImageUrl = "https://via.placeholder.com/300x200?text=Plant",
                CareInstructions = "Mock care instructions: Water when soil is dry",
                ExternalApiId = $"mock-{Guid.NewGuid()}"
            };
        }

        public async Task<Plant?> CreateOrUpdatePlantAsync(Plant plant)
        {
            var existingPlant = await _context.Plants
                .FirstOrDefaultAsync(p => p.ExternalApiId == plant.ExternalApiId);

            if (existingPlant != null)
            {
                existingPlant.Name = plant.Name;
                existingPlant.ScientificName = plant.ScientificName;
                existingPlant.Description = plant.Description;
                existingPlant.ImageUrl = plant.ImageUrl;
                existingPlant.CareInstructions = plant.CareInstructions;
                existingPlant.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                
                return existingPlant;
            }

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            
            return plant;
        }

        public async Task<IEnumerable<Plant>> GetAllPlantsAsync()
        {
            return await _context.Plants.ToListAsync();
        }

        public async Task<UserPlant?> AddPlantToUserAsync(int userId, int plantId, string nickname)
        {
            // check if user already owns the plant
            var existingUserPlant = await _context.UserPlants
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PlantId == plantId);

            // user already has plant
            if (existingUserPlant != null) return null; 

            // add a new plant
            var userPlant = new UserPlant
            {
                UserId = userId,
                PlantId = plantId,
                PlantNickname = nickname,
                HealthStatus = "Good"
            };

            _context.UserPlants.Add(userPlant);
            await _context.SaveChangesAsync();

            return await _context.UserPlants
                .Include(up => up.Plant)
                .FirstOrDefaultAsync(up => up.Id == userPlant.Id);
        }

        public async Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId)
        {
            return await _context.UserPlants
                .Include(up => up.Plant)
                .Where(up => up.UserId == userId)
                .ToListAsync();
        }

        public async Task<UserPlant?> UpdateUserPlantAsync(int userId, int userPlantId, UserPlant updates)
        {
            var userPlant = await _context.UserPlants
                .Include(up => up.Plant)
                .FirstOrDefaultAsync(up => up.Id == userPlantId && up.UserId == userId);

            if (userPlant == null) return null;

            userPlant.PlantNickname = updates.PlantNickname ?? userPlant.PlantNickname;
            userPlant.HealthStatus = updates.HealthStatus ?? userPlant.HealthStatus;
            userPlant.LastWatered = updates.LastWatered ?? userPlant.LastWatered;
            userPlant.LastFertilized = updates.LastFertilized ?? userPlant.LastFertilized;
            userPlant.Notes = updates.Notes ?? userPlant.Notes;
            userPlant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            return userPlant;
        }
    }
}