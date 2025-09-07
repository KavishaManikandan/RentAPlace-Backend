using System.ComponentModel.DataAnnotations;

namespace RentAPlace.Core.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        [Required] public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;

        [Required] public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        [Required] public int RecipientId { get; set; }
        public User Recipient { get; set; } = null!;

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
