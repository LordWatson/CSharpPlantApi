/*
 * the main difference here is that we define the route path on the controller function as an attribute
 * Symfony does the same thing
 */
using Microsoft.AspNetCore.Mvc;
using PlantAPI.DTOs;
using PlantAPI.Services;

namespace PlantAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);

            if(result == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if(result == null)
                return BadRequest(new { message = "Email already exists" });

            return CreatedAtAction(nameof(Register), result);
        }
    }
}