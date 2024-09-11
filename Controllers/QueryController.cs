using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
namespace Apptivate_UQMS_WebApp.Controllers
{
    public class QueryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QueryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> CreateQuery()
        {
            return View("NewQuery/CreateQuery");
        }

      
        // Action for Academic issues
        [HttpGet]
        public IActionResult AcademicQuery()
        {
            return View("NewQuery/AcademicQuery");
        }

        // Action for Administrative issues
        [HttpGet]
        public IActionResult AdministrativeQuery()
        {

            return View("NewQuery/AdministrativeQuery");
        }

        [HttpPost]
        public IActionResult SubmitAcademicQuery(QueryModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the academic query here (e.g., save to database)
                return RedirectToAction("Confirmation");
            }
            return View("Academic");
        }

        [HttpPost]
        public IActionResult SubmitAdministrativeQuery(QueryModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the administrative query here (e.g., save to database)
                return RedirectToAction("Confirmation");
            }
            return View("Administrative");
        }

        public IActionResult Confirmation()
        {
            return View();
        }



        [HttpGet]
        public IActionResult SubmitQuery()
        {
            var model = new SubmitQueryViewModel
            {
                Departments = _context.Departments.ToList(),
                Courses = _context.Courses.ToList(),
                Modules = _context.Modules.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuery(SubmitQueryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var query = new Query
                {
                    StudentID = model.StudentID,  // Ideally, you would get this from the logged-in user
                    Category = model.Category,
                    DepartmentID = model.DepartmentID,
                    CourseID = model.CourseID,
                    ModuleID = model.ModuleID,
                    Year = model.Year,
                    Status = "Pending",
                    SubmissionDate = DateTime.Now
                };

                _context.Queries.Add(query);
                await _context.SaveChangesAsync();

                return RedirectToAction("QuerySubmitted");
            }

            // Re-populate dropdowns if model is invalid
            model.Departments = _context.Departments.ToList();
            model.Courses = _context.Courses.ToList();
            model.Modules = _context.Modules.ToList();
            return View(model);
        }


        // Action for displaying all tickets
        public ActionResult AllTickets()
        {
            // var allTickets = _ticketService.GetAllTickets(); // Uncomment and use your service to fetch tickets
            return PartialView("Query/QueryOverview/AllTickets.cshtml");
        }

        // Action for displaying new tickets
        public ActionResult NewTickets()
        {
            // var newTickets = _ticketService.GetTicketsByStatus("New");
            return PartialView("Views/Queries/QueryOverview/NewTickets.cshtml");
        }

        // Action for displaying ongoing tickets
        public ActionResult OnGoingTickets()
        {
            // var onGoingTickets = _ticketService.GetTicketsByStatus("On-Going");
            return PartialView("QueryOverview/OnGoingTickets");
        }

        // Action for displaying resolved tickets
        public ActionResult ResolvedTickets()
        {
            // var resolvedTickets = _ticketService.GetTicketsByStatus("Resolved");
            return PartialView("QueryOverview/ResolvedTickets");
        }

        public IActionResult Queries()
        {
            return View();
        }


        public IActionResult QuerySubmitted()
        {
            return View();
        }
    }

}
