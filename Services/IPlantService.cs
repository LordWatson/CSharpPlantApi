/*
 * Interface class
 * no different to how we'd use an interface in Laravel
 *
 * * .NET best practices seem to be to always create an Interface when creating a Service
 *
 * naming conventions for .NET use an I{ServiceName}
 * then a service extending the Interface would look like:
 * public class PlantService : IPlantService
 */
using PlantAPI.Models;

namespace PlantAPI.Services
{
    public interface IPlantService
    {
        /*
         * nullable reference type
         * similar to Laravel when we'd do something like:
         * $plant?->nickname;
         *
         * the structure of the lines below follows:
         * return type -> function -> parameters
         *
         * Task here represents an Async operation
         * Laravel usually just handles this for us so in an interface we'd probably see:
         * public function getUserById(int $userId) : User
         */
        Task<Plant?> GetPlantFromExternalApiAsync(string plantName);
        Task<Plant?> CreateOrUpdatePlantAsync(Plant plant);
        Task<IEnumerable<Plant>> GetAllPlantsAsync();
        Task<UserPlant?> AddPlantToUserAsync(int userId, int plantId, string nickname);
        Task<IEnumerable<UserPlant>> GetUserPlantsAsync(int userId);
        Task<UserPlant?> UpdateUserPlantAsync(int userId, int userPlantId, UserPlant updates);
    }
}