using Microsoft.AspNetCore.Mvc;
using RentAPlace.Core.Models;
using RentAPlace.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RentAPlace.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OwnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all properties owned by this owner
        [Authorize(Roles = "Owner")]
        [HttpGet("{ownerId}/properties")]
        public IActionResult GetOwnerProperties(int ownerId)
        {
            var owner = _context.Users.FirstOrDefault(u => u.UserId == ownerId && u.Role == "Owner");
            if (owner == null) return NotFound("Owner not found.");

            var properties = _context.Properties
                .Where(p => p.OwnerId == ownerId)
                .ToList();

            return Ok(properties);
        }

        // Add new property
        [Authorize(Roles = "Owner")]
        [HttpPost("{ownerId}/properties")]
        public IActionResult AddProperty(int ownerId, Property property)
        {
            var owner = _context.Users.FirstOrDefault(u => u.UserId == ownerId && u.Role == "Owner");
            if (owner == null) return NotFound("Owner not found.");

            property.OwnerId = ownerId;
            _context.Properties.Add(property);
            _context.SaveChanges();

            return Ok(property);
        }

        // Update property
        [Authorize(Roles = "Owner")]
        [HttpPut("properties/{propertyId}")]
        public IActionResult UpdateProperty(int propertyId, Property updated)
        {
            var property = _context.Properties.Find(propertyId);
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

        // Delete property
        [Authorize(Roles = "Owner")]
        [HttpDelete("properties/{propertyId}")]
        public IActionResult DeleteProperty(int propertyId)
        {
            var property = _context.Properties.Find(propertyId);
            if (property == null) return NotFound();

            _context.Properties.Remove(property);
            _context.SaveChanges();

            return Ok();
        }

        // View reservations for owner’s properties
        [Authorize(Roles = "Owner")]
        [HttpGet("{ownerId}/reservations")]
        public async Task<IActionResult> GetOwnerReservations(int ownerId)
        {
            var reservations = await _context.Reservations
         .Include(r => r.Property)
         .Include(r => r.User)
         .Where(r => r.Property.OwnerId == ownerId)
         .ToListAsync();

            var response = reservations.Select(r => new
            {
                r.ReservationId,
                r.CheckIn,
                r.CheckOut,
                r.Guests,
                r.Status,

                // property info
                Property = new
                {
                    r.Property.PropertyId,
                    r.Property.Title,
                    r.Property.Location,
                    r.Property.PricePerNight,
                    r.Property.ImagePath
                },

                //  user info
                User = new
                {
                    r.User.UserId,
                    r.User.FullName,
                    r.User.Email,
                    r.User.Role
                }
            });

            return Ok(response);
        }

        // View messages for owner’s properties
        [Authorize(Roles = "Owner")]
        [HttpGet("{ownerId}/messages")]
        public async Task<IActionResult> GetOwnerMessages(int ownerId)
        {
            var messages = await _context.Messages
        .Include(m => m.Property)
        .Include(m => m.Sender)
        .Include(m => m.Recipient)
        .Where(m => m.Property.OwnerId == ownerId)   // filter by owner
        .ToListAsync();

    var response = messages.Select(m => new
    {
        m.MessageId,
        m.PropertyId,
        Property = m.Property == null ? null : new
        {
            m.Property.PropertyId,
            m.Property.Title,
            m.Property.Location,
            m.Property.PropertyType,
            m.Property.PricePerNight,
            m.Property.ImagePath
        },
        m.SenderId,
        Sender = m.Sender == null ? null : new
        {
            m.Sender.UserId,
            m.Sender.FullName,
            m.Sender.Email,
            m.Sender.Role
        },
        m.RecipientId,
        Recipient = m.Recipient == null ? null : new
        {
            m.Recipient.UserId,
            m.Recipient.FullName,
            m.Recipient.Email,
            m.Recipient.Role
        },
        m.Content,
        m.CreatedAt,
        m.IsRead
    });

    return Ok(response);
        }
    }
}
