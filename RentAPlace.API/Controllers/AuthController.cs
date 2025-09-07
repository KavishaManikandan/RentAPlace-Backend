using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentAPlace.Core.DTOs;
using RentAPlace.Core.Models;
using RentAPlace.Api.Security;
using RentAPlace.Infrastructure.Data;

namespace RentAPlace.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenService _jwt;

        public AuthController(ApplicationDbContext context, IJwtTokenService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("Email already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = _jwt.CreateToken(user);
            return Ok(new { token, user = new { user.UserId, user.FullName, user.Email, user.Role } });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                return Unauthorized("Invalid email or password.");

            var token = _jwt.CreateToken(user);
            return Ok(new { token, user = new { user.UserId, user.FullName, user.Email, user.Role } });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            return Ok(new { user.UserId, user.FullName, user.Email, user.Role });
        }
    }
}
