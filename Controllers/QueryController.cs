using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
namespace Apptivate_UQMS_WebApp.Controllers
{
    public class QueryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QueryController> _logger;  // Inject ILogger

        public QueryController(ApplicationDbContext context, ILogger<QueryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Action to render the CreateQuery page
        [HttpGet]
        public async Task<IActionResult> CreateQuery()
        {
            return View("NewQuery/CreateQuery");
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
        public async Task<IActionResult> SubmitAcademicQuery(Query model)
        {
            if (ModelState.IsValid)
            {
                var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

                if (firebaseUid == null)
                {
                    _logger.LogError("User not logged in.");
                    return RedirectToAction("Login", "Account");
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

                if (user == null)
                {
                    _logger.LogError("User with FirebaseUID {FirebaseUID} not found.", firebaseUid);
                    return NotFound();
                }

                // Fetch StudentID, DepartmentID, and CourseID by joining tables
                var studentDetailQuery = await (
         from student in _context.StudentDetails
         join department in _context.Departments
         on student.Department equals department.DepartmentName into deptGroup
         from department in deptGroup.DefaultIfEmpty() // Ensure to handle cases where no department matches
         join course in _context.Courses
         on student.Course equals course.CourseCode into courseGroup // Use CourseCode for matching
         from course in courseGroup.DefaultIfEmpty() // Ensure to handle cases where no course matches
         where student.UserID == user.UserID
         select new
         {
             student.StudentID,
             DepartmentID = department != null ? department.DepartmentID : (int?)null,
             CourseID = course != null ? course.CourseID : (int?)null,
             student.Year
         }).FirstOrDefaultAsync();


                if (studentDetailQuery == null)
                {
                    _logger.LogError("Student details not found for UserID {UserID}. Check if UserID matches existing records and ensure Department and Course names are correct.", user.UserID);
                    return NotFound();
                }

                // Log the values for debugging
                _logger.LogInformation("Query submission details:");
                _logger.LogInformation("StudentID: {StudentID}, DepartmentID: {DepartmentID}, CourseID: {CourseID}",
                    studentDetailQuery.StudentID, studentDetailQuery.DepartmentID, studentDetailQuery.CourseID);
                _logger.LogInformation("QueryTypeID: {QueryTypeID}, CategoryID: {CategoryID}, Year: {Year}",
                    model.QueryTypeID, model.CategoryID, studentDetailQuery.Year);

                try
                {
                    var query = new Query
                    {
                        StudentID = studentDetailQuery.StudentID,
                        QueryTypeID = model.QueryTypeID,
                        CategoryID = model.CategoryID,
                        DepartmentID = studentDetailQuery.DepartmentID ?? 0, // Handle null properly
                        CourseID = studentDetailQuery.CourseID ?? 0, // Handle null properly
                        //ModuleID = model.ModuleID ?? 0,
                        Year = studentDetailQuery.Year,
                        Status = "Pending",
                        SubmissionDate = DateTime.Now
                    };

                    _context.Queries.Add(query);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Query with ID {QueryID} successfully submitted.", query.QueryID);

                    return RedirectToAction("CreateQuery"); // Redirect to CreateQuery view after submission
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while submitting the query.");
                    return View("Error"); // Redirect to an error view or page
                }
            }

            // Log detailed ModelState errors
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("ModelState error for key {Key}: {ErrorMessage}", state.Key, error.ErrorMessage);
                    }
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
