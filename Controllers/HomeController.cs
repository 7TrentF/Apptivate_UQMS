using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Apptivate_UQMS_WebApp.Controllers
{

   // [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //guyyufgyuvyvvgvghvghvgh
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult KnowledgeBaseFAQ()
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
        public IActionResult Error(int? statusCode = null)
        {
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (statusCode.HasValue)
            {
                switch (statusCode.Value)
                {
                    case 404:
                        errorModel.ErrorTitle = "Page Not Found";
                        errorModel.ErrorMessage = "Sorry, the page you requested could not be found.";
                        break;
                    case 500:
                        errorModel.ErrorTitle = "Server Error";
                        errorModel.ErrorMessage = "Sorry, something went wrong on our end.";
                        break;
                    default:
                        errorModel.ErrorTitle = "Error";
                        errorModel.ErrorMessage = "An unexpected error occurred.";
                        break;
                }
                errorModel.StatusCode = statusCode.Value;
            }

            _logger.LogError($"Error occurred. Status Code: {statusCode}, RequestId: {errorModel.RequestId}");
            return View(errorModel);
        }
    }
}
