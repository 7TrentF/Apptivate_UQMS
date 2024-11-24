
namespace Apptivate_UQMS_WebApp.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Apptivate_UQMS_WebApp.Models.Account;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        public int? ConversationID { get; set; } // Nullable if it's optional

        [Required]
        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        // Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}