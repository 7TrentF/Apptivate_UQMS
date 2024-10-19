namespace Apptivate_UQMS_WebApp.Models
{
    public class ChatUserViewModel
    {
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public int UnreadCount { get; set; }
    }
}
