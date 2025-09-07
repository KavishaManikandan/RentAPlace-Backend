using Microsoft.AspNetCore.Mvc;
using RentAPlace.Infrastructure.Data;
using RentAPlace.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace RentAPlace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(Property dto)
        {
            var owner = await _context.Users.FindAsync(dto.OwnerId);
            if (owner == null)
            {
                return BadRequest(new { message = "Owner not found" });
            }

            var property = new Property
            {
                OwnerId = dto.OwnerId,
                Owner = owner,
                Title = dto.Title,
                Location = dto.Location,
                PropertyType = dto.PropertyType,
                Features = dto.Features,
                Rating = dto.Rating,
                ImagePath = dto.ImagePath,
                PricePerNight = dto.PricePerNight
            };

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return Ok(property);
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Properties.ToList());

        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            var property = await _context.Properties
                .Include(p => p.Owner)
                .Include(p => p.Reservations)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
            {
                return NotFound(new { message = $"Property with ID {id} not found." });
            }

            return Ok(property);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Property updated)
        {
            var property = _context.Properties.Find(id);
            if (property == null) return NotFound();

            property.Title = updated.Title;
            property.Location = updated.Location;
            property.PropertyType = updated.PropertyType;
            property.Features = updated.Features;
            property.Rating = updated.Rating;
            property.ImagePath = updated.ImagePath;
            property.PricePerNight = updated.PricePerNight;

            _context.SaveChanges();
            return Ok(property);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var property = await _context.Properties
        .Include(p => p.Reservations)
        .FirstOrDefaultAsync(p => p.PropertyId == id);
            if (property == null) return NotFound();

            if (property.Reservations.Any())
            {
                return BadRequest("Cannot delete property with existing reservations.");
            }

            _context.Properties.Remove(property);
            _context.SaveChanges();
            return Ok();
        }
    }
}
