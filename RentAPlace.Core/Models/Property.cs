using System.Text.Json.Serialization;

namespace RentAPlace.Core.Models
{
    
    
    public class Property
{
    public int PropertyId { get; set; }
    public int OwnerId { get; set; }
    public User? Owner { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string Features { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string? ImagePath { get; set; }
    public int PricePerNight  { get; set; }

        // Navigation
    [JsonIgnore]
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();  //  added
}

    }

