namespace RentAPlace.API.Dtos
{
    public class ReservationDto
    {
        public int PropertyId { get; set; }
        public int UserId { get; set; }   // pass logged-in user id
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Guests { get; set; }
    }

    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }
}