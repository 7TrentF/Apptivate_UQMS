using Microsoft.AspNetCore.Mvc;

namespace Apptivate_UQMS_WebApp.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
