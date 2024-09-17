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
using static Apptivate_UQMS_WebApp.Models.Account;
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
            return View("NewQuery/QuerySubmitted");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitAdministrativeQuery(Query model, IFormFile uploadedFile)
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
                    return RedirectToAction("QuerySubmitted");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                    ModelState.AddModelError("", ex.Message);
                }
            }

            _logger.LogWarning("Model state is invalid for the query submission.");
            return View("NewQuery/QuerySubmitted");
        }



        public IActionResult Confirmation()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> AllTickets()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            _logger.LogInformation("FirebaseUID: {0}", firebaseUid ?? "null");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            _logger.LogInformation("User found: {0}", user?.UserID.ToString() ?? "null");

            if (user == null)
            {
                _logger.LogError("User not found.");
                return RedirectToAction("Error", "Home");
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);
            _logger.LogInformation("Student details found: {0}", studentDetail?.StudentID.ToString() ?? "null");

            if (studentDetail == null)
            {
                _logger.LogError("Student details not found.");
                return RedirectToAction("Error", "Home");
            }

            _logger.LogInformation("StudentID: {0}", studentDetail.StudentID);

            // Fetch user queries based on the student's ID
            var userQueries = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID)
                .ToListAsync(); // Assuming QueryModel

            _logger.LogInformation("Number of queries found: {0}", userQueries.Count);

            if (userQueries.Count == 0)
            {
                _logger.LogWarning("No queries found for StudentID: {0}", studentDetail.StudentID);
            }

            return PartialView("QueryOverview/AllTickets", userQueries);
        }



        // Action for displaying new tickets
        public ActionResult NewTickets()
        {
            // var newTickets = _ticketService.GetTicketsByStatus("New");
            return PartialView("QueryOverview/NewTickets");
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
