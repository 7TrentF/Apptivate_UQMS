//using Apptivate_UQMS_WebApp.Data;
//using Apptivate_UQMS_WebApp.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;


namespace Apptivate_UQMS_WebApp.Controllers
{
    internal class ChatMessage
    {
        public string User { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public string AttachmentPath { get; set; }
    }
}