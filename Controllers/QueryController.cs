using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using FirebaseAdmin;
using Google.Cloud.Storage.V1;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Auth.OAuth2;
using Apptivate_UQMS_WebApp.Services;
namespace Apptivate_UQMS_WebApp.Controllers
{
    public class QueryController : Controller
    {
        private readonly ApplicationDbContext _context; // Inject _context
        private readonly ILogger<QueryController> _logger;  // Inject ILogger 
        private readonly IQueryService _queryService;  // Inject IQueryService

        public QueryController(IQueryService queryService, ApplicationDbContext context, ILogger<QueryController> logger)
        {
            _queryService = queryService;
            _context = context;
            _logger = logger;
        }

        // Action to render the CreateQuery page
        [HttpGet]
        public Task<IActionResult> CreateQuery()
        {
            return Task.FromResult<IActionResult>(View("NewQuery/CreateQuery"));
        }

        [HttpGet]
        public async Task<IActionResult> AcademicQuery(int queryTypeId)
        {
            _logger.LogInformation($"AcademicQuery called with queryTypeId: {queryTypeId}");

            // Retrieve the Firebase UID from session
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Fetch the user and student details
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                _logger.LogError("User not found.");
                return NotFound();
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                _logger.LogError("Student details not found.");
                return NotFound();
            }

            // Fetch the query type and categories
            var queryType = await _context.QueryTypes
                                          .Include(qt => qt.QueryCategories)
                                          .FirstOrDefaultAsync(qt => qt.QueryTypeID == queryTypeId);

            if (queryType == null)
            {
                return NotFound();
            }

            ViewBag.QueryTypeID = queryTypeId;
            ViewBag.QueryCategories = queryType.QueryCategories;
            ViewBag.StudentDetail = studentDetail;  // Pass the student details to the view
            ViewBag.StudentID = studentDetail.StudentID;
            ViewBag.Department = studentDetail.Department;
            ViewBag.Course = studentDetail.Course;
            ViewBag.Year = studentDetail.Year;
           


            return View("NewQuery/AcademicQuery");
        }

        [HttpGet]
        public async Task<IActionResult> AdministrativeQuery(int queryTypeId)
        {
            _logger.LogInformation($"AdministrativeQuery called with queryTypeId: {queryTypeId}");

            // Retrieve the Firebase UID from session
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Fetch the user and student details
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                _logger.LogError("User not found.");
                return NotFound();
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                _logger.LogError("Student details not found.");
                return NotFound();
            }

            // Fetch the query type and categories
            var queryType = await _context.QueryTypes
                                          .Include(qt => qt.QueryCategories)
                                          .FirstOrDefaultAsync(qt => qt.QueryTypeID == queryTypeId);

            if (queryType == null)
            {
                return NotFound();
            }

            ViewBag.QueryTypeID = queryTypeId;
            ViewBag.QueryCategories = queryType.QueryCategories;
            ViewBag.StudentDetail = studentDetail;  // Pass the student details to the view

            return View("NewQuery/AdministrativeQuery");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAcademicQuery(Query model, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

                if (firebaseUid == null)
                {
                    _logger.LogError("User not logged in.");
                    return RedirectToAction("Login", "Account");
                }
                // Check length of the Description field
                if (model.Description.Length > 150)
                {
                    ModelState.AddModelError("Description", "Description cannot be longer than 150 characters.");
                    return View("NewQuery/AcademicQuery"); // Return the form if validation fails
                }

                try
                {
                    await _queryService.SubmitAcademicQueryAsync(model, uploadedFile, firebaseUid);
                    return RedirectToAction("CreateQuery");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                    ModelState.AddModelError("", ex.Message);
                }
            }

            _logger.LogWarning("Model state is invalid for the query submission.");
            return View("NewQuery/CreateQuery");
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
        

        /*

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
                   QueryTypeID = model.QueryTypeID,  // Ideally, you would get this from the logged-in user
                  // Category = model.Category,
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

       */

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
            return View("NewQuery/QuerySubmitted");
        }
    }

}
