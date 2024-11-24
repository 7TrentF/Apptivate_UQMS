using static Apptivate_UQMS_WebApp.Models.Account;
using System;
using System.ComponentModel.DataAnnotations;

namespace Apptivate_UQMS_WebApp.Models
{
    public class SystemActivity
    {
        [Key]
        public int ActivityID { get; set; }
        public int UserID { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Navigation property
        public User User { get; set; }
    }
}
