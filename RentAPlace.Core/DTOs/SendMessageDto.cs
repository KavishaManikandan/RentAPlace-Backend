namespace RentAPlace.API.Dtos
{
    public class SendMessageDto
    {
        public int PropertyId { get; set; }
        public int RecipientId { get; set; } // other user (owner or renter)
        public string Content { get; set; } = string.Empty;
    }
}