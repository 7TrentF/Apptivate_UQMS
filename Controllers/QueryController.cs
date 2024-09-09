using Apptivate_UQMS_WebApp.Data;
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


        public ActionResult AllTickets()
        {
           // var allTickets = _ticketService.GetAllTickets(); // Assuming you have a service to fetch all tickets
            return PartialView();
        }

        public ActionResult NewTickets()
        {
           // var newTickets = _ticketService.GetTicketsByStatus("New");
            return PartialView();
        }

        public ActionResult OnGoingTickets()
        {
           // var onGoingTickets = _ticketService.GetTicketsByStatus("On-Going");
            return PartialView();
        }

        public ActionResult ResolvedTickets()
        {
          //  var resolvedTickets = _ticketService.GetTicketsByStatus("Resolved");
            return PartialView();
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
