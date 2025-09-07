using System.Text.Json.Serialization;

namespace RentAPlace.Core.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int Guests { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Navigation
        [JsonIgnore]
        public User? User { get; set; } = null;
        [JsonIgnore]
        public Property? Property { get; set; } = null;
    }
}
