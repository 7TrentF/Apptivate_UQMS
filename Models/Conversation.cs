using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationID { get; set; }

        public string User1ID { get; set; }
        public string User2ID { get; set; }

        // Navigation property to link messages to the conversation
        public ICollection<Message> Messages { get; set; }
    }
}