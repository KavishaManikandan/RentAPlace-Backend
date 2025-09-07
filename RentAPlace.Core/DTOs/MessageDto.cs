namespace RentAPlace.API.Dtos
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int PropertyId { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
