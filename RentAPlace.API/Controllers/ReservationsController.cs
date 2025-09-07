using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.API.Dtos;
using RentAPlace.Infrastructure.Data;
using RentAPlace.Core.Models;
using RentAPlace.API.Services;

namespace RentAPlace.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> CreateReservation(ReservationDto dto, [FromServices] IEmailService emailService)
        {
            //check property exists
            var property = await _context.Properties
            .Where(p => p.PropertyId == dto.PropertyId)
            .Select(p => new
            {
                p.PropertyId,
                p.Owner,
                p.Title,
                Reservations = p.Reservations.Select(r => new { r.CheckIn, r.CheckOut })
            })
            .FirstOrDefaultAsync();


            if (property == null)
                return NotFound(new { message = "Property not found." });

            // check availability (no overlap)
            bool isBooked = property.Reservations.Any(r =>
                dto.CheckIn < r.CheckOut && dto.CheckOut > r.CheckIn);

            if (isBooked)
                return BadRequest(new { message = "Property is already booked for these dates." });

            //create reservation
            var reservation = new Reservation
            {
                PropertyId = dto.PropertyId,
                UserId = dto.UserId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Guests = dto.Guests
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var body = $@"
            <h3>New Reservation</h3>
            <p>Your property <b>{property.Title}</b> has been reserved.</p>
            <p><b>Check-In:</b> {dto.CheckIn:yyyy-MM-dd}</p>
            <p><b>Check-Out:</b> {dto.CheckOut:yyyy-MM-dd}</p>
            <p><b>Guests:</b> {dto.Guests}</p>
            <p>Please login to the website and confirm booking!!</p>
        ";

            await emailService.SendEmailAsync(
                "kavishatnj@gmail.com",       //Change the email of owner in future 
                "New Property Reservation",
                body
            );

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.ReservationId }, reservation);
        }

        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Property)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
                return NotFound();

            return Ok(reservation);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto status)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.Status = status.Status; // e.g., "Confirmed"
            await _context.SaveChangesAsync();

            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
