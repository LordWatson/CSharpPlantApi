using System.ComponentModel.DataAnnotations;

/*
 * these are similar to FormRequests in Laravel 
 * this file also handles the response formatting - @see AuthResponse
 * 
 * .NET will put multiple "Request" classes into one file
 * where Laravel we'd use a file per-request type - @see LoginRequest and RegisterRequest are in the same file
 */
namespace PlantAPI.DTOs
{
    public class LoginRequest
    {
        /*
         * .NET uses attributes rather than validation rules in Laravel 
         */
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
    
    public class RegisterRequest
    {
        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required, StringLength(50), MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
    
    /*
     * Laravel we'd use a Resource class, usually extending JsonResource
     */
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}