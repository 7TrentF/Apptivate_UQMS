using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Apptivate_UQMS_WebApp.Controllers
{

    // [Authorize]
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            //_logger = logger;
        }


        //guyyufgyuvyvvgvghvghvgh
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public ActionResult AllTickets()
        {
            return PartialView();
        }

        public ActionResult NewTickets()
        {
            return PartialView();
        }

        public ActionResult OnGoingTickets()
        {
            return PartialView();
        }

        public ActionResult ResolvedTickets()
        {
            return PartialView();
        }

        public IActionResult Queries()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}