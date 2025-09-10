/*
 * the main difference here is that we define the route path on the controller function as an attribute
 * Symfony does the same thing
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using PlantAPI.Models;
using PlantAPI.Services;
using PlantAPI.DTOs;

namespace PlantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all endpoints
    public class PlantsController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantsController(IPlantService plantService)
        {
            _plantService = plantService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlants()
        {
            var plants = await _plantService.GetAllPlantsAsync();
            
            return Ok(plants);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchPlant([FromBody] SearchPlantRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.PlantName))
                return BadRequest(new { message = "Plant name is required" });

            var plant = await _plantService.GetPlantFromExternalApiAsync(request.PlantName);

            if(plant == null)
                return NotFound(new { message = "Plant not found" });

            // save the plant to our database
            plant = await _plantService.CreateOrUpdatePlantAsync(plant);

            return Ok(plant);
        }

        [HttpPost("{plantId}/add-to-collection")]
        public async Task<IActionResult> AddPlantToCollection(int plantId, [FromBody] AddPlantRequest request)
        {
            var userId = GetCurrentUserId();
            var userPlant = await _plantService.AddPlantToUserAsync(userId, plantId, request.Nickname);

            if(userPlant == null)
                return BadRequest(new { message = "Plant already in your collection" });

            return CreatedAtAction(nameof(GetUserPlants), userPlant);
        }

        [HttpGet("my-plants")]
        public async Task<IActionResult> GetUserPlants()
        {
            var userId = GetCurrentUserId();
            var userPlants = await _plantService.GetUserPlantsAsync(userId);
            
            return Ok(userPlants);
        }

        [HttpPut("my-plants/{userPlantId}")]
        public async Task<IActionResult> UpdateUserPlant(int userPlantId, [FromBody] UpdateUserPlantRequest request)
        {
            var userId = GetCurrentUserId();
            var updates = new UserPlant
            {
                PlantNickname = request.PlantNickname,
                HealthStatus = request.HealthStatus,
                LastWatered = request.LastWatered,
                LastFertilized = request.LastFertilized,
                Notes = request.Notes
            };

            var userPlant = await _plantService.UpdateUserPlantAsync(userId, userPlantId, updates);

            if(userPlant == null)
                return NotFound(new { message = "Plant not found in your collection" });

            return Ok(userPlant);
        }
    }

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