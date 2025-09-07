using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Infrastructure.Data;
using RentAPlace.API.Dtos;
using RentAPlace.Core.Models;
using System.Security.Claims;

namespace RentAPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("thread")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetThread([FromQuery] int propertyId, [FromQuery] int otherUserId)
        {
            var me = GetUserId();
            if (me == null) return Unauthorized();

            var msgs = await _context.Messages
                .AsNoTracking()
                .Where(m => m.PropertyId == propertyId &&
                           ((m.SenderId == me && m.RecipientId == otherUserId) ||
                            (m.SenderId == otherUserId && m.RecipientId == me)))
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    MessageId = m.MessageId,
                    PropertyId = m.PropertyId,
                    SenderId = m.SenderId,
                    RecipientId = m.RecipientId,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt,
                    IsRead = m.IsRead
                })
                .ToListAsync();

            return Ok(msgs);
        }

        
        [HttpPost]
        public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto)
        {
            var me = GetUserId();
            if (me == null) return Unauthorized();

           
            var propertyExists = await _context.Properties.AnyAsync(p => p.PropertyId == dto.PropertyId);
            if (!propertyExists) return NotFound(new { message = "Property not found." });

            var recipientExists = await _context.Users.AnyAsync(u => u.UserId == dto.RecipientId);
            if (!recipientExists) return NotFound(new { message = "Recipient not found." });

            var msg = new Message
            {
                PropertyId = dto.PropertyId,
                SenderId = me.Value,
                RecipientId = dto.RecipientId,
                Content = dto.Content.Trim()
            };

            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();

            var result = new MessageDto
            {
                MessageId = msg.MessageId,
                PropertyId = msg.PropertyId,
                SenderId = msg.SenderId,
                RecipientId = msg.RecipientId,
                Content = msg.Content,
                CreatedAt = msg.CreatedAt,
                IsRead = msg.IsRead
            };

            return CreatedAtAction(nameof(GetThread), new { propertyId = dto.PropertyId, otherUserId = dto.RecipientId }, result);
        }

        private int? GetUserId()
        {
          
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("userId");
            return claim != null && int.TryParse(claim.Value, out var id) ? id : null;
        }
    }
}
