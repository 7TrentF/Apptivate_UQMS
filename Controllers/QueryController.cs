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
using Microsoft.AspNetCore.Authorization;
using Apptivate_UQMS_WebApp.Extentions; // Add this using directive
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using QueryStatus = Apptivate_UQMS_WebApp.Models.QueryModel.QueryStatus;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize(Roles = "Student")]
    public class QueryController : Controller
    {
        private readonly IEmailService _emailService;

        private readonly ApplicationDbContext _context; // Inject _context
        private readonly ILogger<QueryController> _logger;  // Inject ILogger 
        private readonly IQueryService _queryService;  // Inject IQueryService

        public QueryController(IQueryService queryService, ApplicationDbContext context, ILogger<QueryController> logger, IEmailService emailService)
        {
            _queryService = queryService;
            _context = context;
            _logger = logger;
            _emailService = emailService;

        }

        // Action to render the CreateQuery page
        [HttpGet]
        public Task<IActionResult> CreateQuery()
        {
            return Task.FromResult<IActionResult>(View("StudentQuery/NewQuery/CreateQuery"));
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

            try
            {
                // Use the service to fetch the academic query details
                var academicQueryDetails = await _queryService.GetAcademicQueryAsync(queryTypeId, firebaseUid);

                // Unpack the data from the service response (since it returns an anonymous object)
                dynamic queryData = academicQueryDetails;

                ViewBag.QueryTypeID = queryData.QueryTypeID;
                ViewBag.QueryCategories = queryData.QueryCategories;
                ViewBag.StudentDetail = queryData.StudentDetail;
                ViewBag.StudentID = queryData.StudentDetail.StudentID;
                ViewBag.Department = queryData.StudentDetail.Department;
                ViewBag.DepartmentID = queryData.StudentDetail.DepartmentID; // Using DepartmentID instead of Department name
                ViewBag.CourseCode = queryData.StudentDetail.CourseCode;         // Using CourseID instead of Course name
                ViewBag.CourseID = queryData.StudentDetail.CourseID;         // Using CourseID instead of Course name
                ViewBag.Year = queryData.StudentDetail.Year;

                return View("StudentQuery/NewQuery/AcademicQuery");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the academic query.");
                return BadRequest("An error occurred while fetching the academic query.");
            }
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

            try
            {
                // Use the service to fetch the academic query details
                var academicQueryDetails = await _queryService.GetAcademicQueryAsync(queryTypeId, firebaseUid);

                // Unpack the data from the service response (since it returns an anonymous object)
                dynamic queryData = academicQueryDetails;

                ViewBag.QueryTypeID = queryData.QueryTypeID;
                ViewBag.QueryCategories = queryData.QueryCategories;
                ViewBag.StudentDetail = queryData.StudentDetail;
                ViewBag.StudentID = queryData.StudentDetail.StudentID;
                ViewBag.Department = queryData.StudentDetail.Department;
                ViewBag.DepartmentID = queryData.StudentDetail.DepartmentID; // Using DepartmentID instead of Department name
                ViewBag.CourseCode = queryData.StudentDetail.CourseCode;         // Using CourseID instead of Course name

                ViewBag.CourseID = queryData.StudentDetail.CourseID;         // Using CourseID instead of Course name
                ViewBag.Year = queryData.StudentDetail.Year;

                return View("StudentQuery/NewQuery/AdministrativeQuery");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the academic query.");
                return BadRequest("An error occurred while fetching the academic query.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitAcademicQuery(QueryDto model, IFormFile uploadedFile)
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
                    return View("StudentQuery/NewQuery/AcademicQuery"); // Return the form if validation fails
                }


               


                try
                {
                    await _queryService.SubmitAcademicQueryAsync(model, uploadedFile, firebaseUid); // Add email parameter



                    return RedirectToAction("CreateQuery");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                    ModelState.AddModelError("", ex.Message);
                }
            }

            _logger.LogWarning("Model state is invalid for the query submission.");
            return View("StudentQuery/NewQuery/QuerySubmitted");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitAdministrativeQuery(QueryDto model, IFormFile uploadedFile)
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
                    return View("StudentQuery/NewQuery/AcademicQuery"); // Return the form if validation fails
                }

                try
                {
                    await _queryService.SubmitAdministrativeQueryAsync(model, uploadedFile, firebaseUid);
                    return RedirectToAction("QuerySubmitted");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                    ModelState.AddModelError("", ex.Message);
                }
            }

            _logger.LogWarning("Model state is invalid for the query submission.");
            return View("StudentQuery/NewQuery/QuerySubmitted");
        }




        // Action to render the dashboard view
        public IActionResult TicketDashboard()
        {
            return View("StudentQuery/TicketDashboard");
        }



        public IActionResult Confirmation()
        {
            return View();
        }


        public async Task<IActionResult> AllTickets()
        {
            // Get Firebase UID from session
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the user from the database based on Firebase UID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Get student details from the database based on user ID
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch all queries for the student
            var userQueries = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID)
                .ToListAsync();

            return PartialView("StudentQuery/QueryOverview/AllTickets", userQueries);
        }


        // Action for displaying new tickets
        public async Task<IActionResult> NewTickets()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            // Fetch new tickets for the student
            var newTickets = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status.Equals(QueryStatus.Ongoing))
                .ToListAsync();

            return PartialView("StudentQuery/QueryOverview/NewTickets", newTickets);
        }

        // Action for displaying ongoing tickets
        public async Task<IActionResult> OnGoingTickets()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            // Fetch ongoing tickets for the student
            var ongoingTickets = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status.Equals(QueryStatus.Ongoing))
                .ToListAsync();

            return PartialView("StudentQuery/QueryOverview/OnGoingTickets", ongoingTickets);
        }

        // Action for displaying resolved tickets
        public async Task<IActionResult> ResolvedTickets()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            // Fetch resolved tickets for the student
            var resolvedTickets = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status.Equals(QueryStatus.Resolved))
                .ToListAsync();

            return PartialView("StudentQuery/QueryOverview/ResolvedTickets", resolvedTickets);
        }

        [HttpGet]
        public async Task<IActionResult> ViewTicket(int queryId)
        {
            // Get Firebase UID from session
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the user from the database based on Firebase UID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Get student details from the database based on user ID
            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Fetch all queries for the student
            var userQueries = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID)
                .ToListAsync();

            var query = await _context.Queries
                             .FirstOrDefaultAsync(q => q.QueryID == queryId);

            return View("StudentQuery/QueryOverview/ViewTicket", query);
        }

        [HttpGet]
        public async Task<IActionResult> ViewResolvedTicket(int queryId)
        {
            _logger.LogInformation("Started ViewResolvedTicket for QueryID: {QueryID}", queryId);

            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
            if (firebaseUid == null)
            {
                _logger.LogWarning("Firebase UID is missing.");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var resolvedTicketDetails = await _queryService.GetResolvedTicketDetails(queryId, firebaseUid);

                if (resolvedTicketDetails == null)
                {
                    _logger.LogError("Resolved ticket details not found for QueryID: {QueryID}", queryId);
                    return NotFound();
                }

                _logger.LogInformation("Successfully retrieved resolved ticket for QueryID: {QueryID}", queryId);
                return View("StudentQuery/QueryOverview/ViewResolvedTicket", resolvedTicketDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving resolved ticket for QueryID: {QueryID}", queryId);
                return BadRequest("An error occurred while fetching the resolved query.");
            }
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int queryId, int rating, string comments, bool isAnonymous)
        {
            // Log the start of feedback submission
            _logger.LogInformation("Started SubmitFeedback for QueryID: {QueryID}, Rating: {Rating}, Anonymous: {IsAnonymous}", queryId, rating, isAnonymous);

            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogWarning("FirebaseUID is null. Redirecting to login.");
                return RedirectToAction("Login", "Account");
            }

            // Fetch user and student details
            var user = await _context.Users
                .Include(u => u.StudentDetails)
                .FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null || !user.StudentDetails.Any())
            {
                _logger.LogError("No user or student details found for FirebaseUID: {FirebaseUID}", firebaseUid);
                return RedirectToAction("Error", "Home");
            }

            var studentId = user.StudentDetails.First().StudentID;
            _logger.LogInformation("Fetched student details for StudentID: {StudentID}", studentId);

            // Fetch the query to ensure it's resolved
            var query = await _context.Queries.FirstOrDefaultAsync(q => q.QueryID == queryId && q.Status == QueryStatus.Resolved);

            if (query == null)
            {
                _logger.LogWarning("No resolved query found for QueryID: {QueryID}", queryId);
                return RedirectToAction("Error", "Home");
            }

            _logger.LogInformation("Resolved query found for QueryID: {QueryID}", queryId);

            // Create the feedback, handling anonymous option
            var feedback = new Feedback
            {
                QueryID = isAnonymous ? (int?)null : queryId,  // If anonymous, set QueryID to null
                StudentID = isAnonymous ? (int?)null : studentId,  // If anonymous, set StudentID to null
                Rating = rating,
                Comments = comments,
                SubmissionDate = DateTime.UtcNow,
                IsAnonymous = isAnonymous
            };

            _logger.LogInformation("Created feedback for QueryID: {QueryID}, IsAnonymous: {IsAnonymous}", isAnonymous ? "Anonymous" : queryId.ToString(), isAnonymous);

            // Add feedback to the database
            _context.Feedback.Add(feedback);
            query.Status = QueryStatus.Closed;  // Mark query as closed after feedback submission
            await _context.SaveChangesAsync();

            _logger.LogInformation("Feedback submitted successfully for QueryID: {QueryID}, StudentID: {StudentID}, Anonymous: {IsAnonymous}", queryId, studentId, isAnonymous);

            return RedirectToAction("ViewResolvedTicket", new { queryId });
        }




        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFeedback(int queryId, int rating, string comments, bool isAnonymous)
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogWarning("FirebaseUID is null. Redirecting to login.");
                return RedirectToAction("Login", "Account");
            }

            var success = await _queryService.SubmitFeedbackAsync(firebaseUid, queryId, rating, comments, isAnonymous);

            if (!success)
            {
                _logger.LogError("Failed to submit feedback for QueryID: {QueryID}", queryId);
                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("ViewResolvedTicket", new { queryId });
        }

        */



        public async Task<IActionResult> Search(string searchQuery, string statusFilter)
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                return RedirectToAction("Error", "Home");
            }



            // Build query based on search criteria
            var queryResults = _context.Queries.AsQueryable();


            if (!string.IsNullOrEmpty(searchQuery))
            {
                queryResults = queryResults.Where(q => q.Description.Contains(searchQuery));
            }

            _logger.LogWarning("searchQuery  " + searchQuery);


            if (!string.IsNullOrEmpty(statusFilter))
            {
                queryResults = queryResults.Where(q => q.Status.Equals(QueryStatus.Ongoing));
            }

            _logger.LogWarning("statusFilter  " + statusFilter);



            queryResults = queryResults.Where(q => q.StudentID == studentDetail.StudentID);

            var queries = await queryResults.ToListAsync();


            _logger.LogWarning(" queries  " + queries);


            return PartialView("StudentQuery/QueryOverview/AllTickets", queries);
        }

        public async Task<IActionResult> FilteredTickets(string dateFilter)
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                return RedirectToAction("Error", "Home");
            }

            // Determine the date range based on the filter
            DateTime startDate, endDate;
            switch (dateFilter)
            {
                case "This Week":
                    startDate = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                    endDate = DateTime.Now.EndOfWeek(DayOfWeek.Sunday);
                    break;
                case "Last Week":
                    var lastWeek = DateTime.Now.AddDays(-7);
                    startDate = lastWeek.StartOfWeek(DayOfWeek.Monday);
                    endDate = lastWeek.EndOfWeek(DayOfWeek.Sunday);
                    break;
                default:
                    startDate = DateTime.MinValue;
                    endDate = DateTime.MaxValue;
                    break;
            }

            // Fetch queries within the specified date range
            var userQueries = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID && q.SubmissionDate >= startDate && q.SubmissionDate <= endDate)
                .ToListAsync();

            return PartialView("StudentQuery/QueryOverview/AllTickets", userQueries);
        }

        public async Task<IActionResult> Queries()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var studentDetail = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            if (studentDetail == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var userQueries = await _context.Queries
                .Where(q => q.StudentID == studentDetail.StudentID)
                .ToListAsync();

            ViewData["InitialTickets"] = userQueries; // Store the initial queries for rendering
            return View("StudentQuery/Queries");
        }

        public IActionResult QuerySubmitted()
        {
            return View("StudentQuery/NewQuery/QuerySubmitted");
        }















    }

}
