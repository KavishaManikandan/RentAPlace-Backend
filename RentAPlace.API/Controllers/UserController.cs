using Microsoft.AspNetCore.Mvc;
using RentAPlace.Infrastructure.Data;
using RentAPlace.Core.Models;

namespace RentAPlace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User login)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            if (user == null) return Unauthorized("Invalid credentials");
            return Ok(user);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Users.ToList());

        [HttpPut("{id}")]
        public IActionResult Update(int id, User updatedUser)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role;

            _context.SaveChanges();
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok();
        }
    }
}
