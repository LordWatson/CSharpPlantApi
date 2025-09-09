/*
 * Interface class
 * no different to how we'd use an interface in Laravel
 *
 * .NET best practices seem to be to always create an Interface when creating a Service
 *
 * naming conventions for .NET use an I{ServiceName}
 * then a service extending the Interface would look like:
 * public class AuthService : IAuthService
 */
using PlantAPI.DTOs;
using PlantAPI.Models;

namespace PlantAPI.Services
{
    public interface IAuthService
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
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RegisterAsync(RegisterRequest request);
        string GenerateJwtToken(User user);
        Task<User?> GetUserByIdAsync(int userId);
    }
}