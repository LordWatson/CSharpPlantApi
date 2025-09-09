/*
 * this is just what you'd expect from a Service class in Laravel
 * it implements the Interface as seen by the syntax:
 * AuthService : IAuthService
 *
 * aside from that, all we're doing is creating the functions we defined in the Interface
 * and writing the business logic in there, to keep it out of the Controller
 * exactly how we do in Laravel
 */
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PlantAPI.Data;
using PlantAPI.DTOs;
using PlantAPI.Models;
using BCrypt.Net;

namespace PlantAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name
            };
        }
        
        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            // check if the user already exists
            if(await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;
            
            // create a new user
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // create the token
            var token = GenerateJwtToken(user);

            // return an AuthResponse class with the new users data
            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name
            };
        }
        
        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}