using Apptivate_UQMS_WebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.QueryModel.QueryResolutions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Apptivate_UQMS_WebApp.Hubs;
using Apptivate_UQMS_WebApp.Services.QueryServices;
using System.Security.Claims;
using Apptivate_UQMS_WebApp.Services;
using QueryStatus = Apptivate_UQMS_WebApp.Models.QueryModel.QueryStatus;
using Query = Apptivate_UQMS_WebApp.Models.QueryModel.Query;


namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffQueryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StaffQueryController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;  // Inject SignalR Hub Context
        private readonly IQueryService _queryService;  // Inject IQueryService
        private readonly IAssignQueryService _assignQueryService;  // Inject IQueryService
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;


        public StaffQueryController(IEmailService emailService, IAssignQueryService assignQueryService, IQueryService queryService, ApplicationDbContext context, ILogger<StaffQueryController> logger, INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _queryService = queryService;
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
            _assignQueryService = assignQueryService;


         
            _emailService = emailService;
            _notificationService = notificationService;
 

        }


    [HttpGet]
        public async Task<IActionResult> QueryDetails(int queryId)
        {
            // Verify that the user is authenticated via session
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogError("Unauthenticated user attempted to access student query details.");
                return Unauthorized();
            }

            try
            {
                // Use the service to fetch the academic query details
                var studentQueryDetails = await _queryService.GetStudentQueryAsync(queryId, firebaseUid);
                var staffQueryAssignment = await _queryService.GetStaffAssignmentQueryDetails(queryId, firebaseUid);

                dynamic StaffQueryAssignmentData = staffQueryAssignment;

                ViewBag.QueryID = StaffQueryAssignmentData.QueryID;
                ViewBag.AssignmentID = StaffQueryAssignmentData.AssignmentID;


                return View("~/Views/Query/StaffQuery/QueryDetails.cshtml", studentQueryDetails); // Load the view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the academic query.");
                return BadRequest("An error occurred while fetching the academic query.");
            }
        }

            // Action to list all queries assigned to the logged-in staff
        [HttpGet]
        public async Task<IActionResult> StaffQueries()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Find the staff member based on the logged-in Firebase UID
            var staff = await _context.StaffDetails
                                      .Include(s => s.User)
                                      .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (staff == null)
            {
                _logger.LogError("Staff not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                return NotFound();
            }

            // Fetch the queries assigned to this staff member
            var queries = await _context.QueryAssignments
                                        .Where(qa => qa.StaffID == staff.StaffID)
                                        .Include(qa => qa.Query)
                                        .ThenInclude(q => q.QueryDocuments) // Include related documents
                                        .ToListAsync();

            return View("~/Views/Query/StaffQuery/StaffQueries.cshtml", queries);

        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedQueries(int staffId)
        {
            var queries = await _context.QueryAssignments
                .Where(qa => qa.StaffID == staffId)
                .Include(qa => qa.Query)
                .Select(qa => new
                {
                    QueryID = qa.Query.QueryID,
                    Description = qa.Query.Description,
                    Priority = qa.Priority,
                    SubmissionDate = qa.Query.SubmissionDate
                })
                .ToListAsync();
            _logger.LogInformation("Assigned Queries:" + queries);

            return Json(queries);
        }


        [HttpPost]
        public async Task<IActionResult> ReAssignQuery(int queryId, int newStaffId)
        {
            _logger.LogInformation("ReAssignQuery initiated with QueryID: {QueryID} and NewStaffID: {NewStaffID}.", queryId, newStaffId);

            try
            {
                // Get the current logged-in staff member's Firebase UID
                var firebaseUid = HttpContext.Session.GetString("FirebaseUID");
                if (firebaseUid == null)
                {
                    _logger.LogError("User not logged in.");
                    return Json(new { success = false, message = "User not logged in." });
                }

                // Delegate to AssignQueryService
                var result = await _assignQueryService.ReassignQueryAsync(queryId, newStaffId, firebaseUid);

                if (result == "Success")
                {
                    return Json(new { success = true, message = "Query successfully reassigned." });
                }
                else
                {
                    return Json(new { success = false, message = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReAssignQuery method.");
                return Json(new { success = false, message = "An error occurred while reassigning the query." });
            }
        }


        // Helper method to send reassignment email
        private async Task SendQueryReassignmentNotificationEmailStaff(Query query, int newStaffId)
        {
            var newStaff = await _context.StaffDetails
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StaffID == newStaffId);

            if (newStaff != null && newStaff.User != null)
            {
                // Implement your email sending logic here
                // Similar to your existing SendQuerySubmissionNotificationEmailStaff method
                await _emailService.SendEmailAsync(
                    newStaff.User.Email,
                    "Query Reassigned",
                    $"A query (#{query.TicketNumber}) has been reassigned to you."
                );
            }
        }




        [HttpGet]
        public async Task<IActionResult> AssignQuery()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Find the staff member based on the logged-in Firebase UID
            var staff = await _context.StaffDetails
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (staff == null)
            {
                _logger.LogError("Staff not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                return NotFound();
            }

            // Fetch queries assigned to the logged-in staff member
            var assignedQueries = await _context.QueryAssignments
                .Where(qa => qa.StaffID == staff.StaffID)
                .Select(qa => new
                {
                    QueryID = qa.Query.QueryID,
                    Description = qa.Query.Description,
                    Priority = qa.Priority,
                    SubmissionDate = qa.Query.SubmissionDate
                })
                .ToListAsync();

            ViewBag.AssignedQueries = assignedQueries;

            var teamMembers = await _assignQueryService.GetTeamMembersAsync(staff.UserID);

            return View("~/Views/Query/StaffQuery/AssignQuery.cshtml", teamMembers);
        }









        // Notify staff when they have new queries to resolve
        public async Task<IActionResult> NotifyNewQueries()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var staff = await _context.StaffDetails
                                      .Include(s => s.User)
                                      .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (staff == null)
            {
                return NotFound("Staff not found.");
            }

            // Check for unresolved queries
            var newQueryCount = await _context.QueryAssignments
                                              .Where(qa => qa.StaffID == staff.StaffID && qa.ResolutionDate == null)
                                              .CountAsync();

            if (newQueryCount > 0)
            {
                // Send a notification to the staff member using SignalR
                await _hubContext.Clients.Group(staff.StaffID.ToString()).SendAsync("ReceiveNotification", $"You have {newQueryCount} new queries.");
            }

            return Json(new { NewQueries = newQueryCount });
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SubmitSolutionToQuery(QueryResolutions model, IFormFile? uploadedFile)
        {
            _logger.LogWarning("SubmitSolutionToQuery hit");

            // Log the incoming model state before validation
            _logger.LogInformation("Incoming Model Data: AssignmentID = {AssignmentID}, QueryID = {QueryID}, Solution = {Solution}, AdditionalNotes = {AdditionalNotes}",
                                   model.AssignmentID, model.QueryID, model.Solution, model.AdditionalNotes);

            // If ModelState is invalid, log the validation errors in detail
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Logging errors:");

                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;

                    foreach (var error in errors)
                    {
                        _logger.LogWarning("ModelState Error - Key: {Key}, Error: {ErrorMessage}, Exception: {Exception}",
                                           key, error.ErrorMessage, error.Exception?.Message);
                    }
                }

                return View("~/Views/Query/StaffQuery/StaffQueries.cshtml");
            }

            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Solution length validation
            if (model.Solution.Length < 10)
            {
                ModelState.AddModelError("Solution", "Solution cannot be less than 10 characters.");
                _logger.LogWarning("Solution validation failed: Less than 10 characters.");
                return View("~/Views/Query/StaffQuery/QueryDetails.cshtml");
            }

            try
            {
                _logger.LogWarning("Attempting SubmitSolutionToQueryAsync method");

                await _queryService.SubmitSolutionToQueryAsync(model, uploadedFile, firebaseUid);

                _logger.LogWarning("SubmitSolutionToQueryAsync method executed successfully.");

                return RedirectToAction("StaffQueries");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                ModelState.AddModelError("", ex.Message);
                return View("~/Views/Query/StaffQuery/StaffQueries.cshtml");
            }
        }

        public async Task<IActionResult> ViewFeedback()
        {
            // Retrieve the currently logged-in staff member based on FirebaseUID
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            // Fetch the user using the FirebaseUID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            // If user not found, handle the case
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Fetch the staff details associated with the user
            var staffDetail = await _context.StaffDetails.FirstOrDefaultAsync(s => s.UserID == user.UserID);

            // If staff detail is not found, return an error message
            if (staffDetail == null)
            {
                return NotFound("Staff details not found.");
            }

            // Fetch all feedback for queries resolved by the staff member and map to FeedbackViewModel
            var studentFeedback = await _context.Feedback
                .Where(f => f.Query.QueryAssignments.Any(qa => qa.StaffID == staffDetail.StaffID))
                .Select(f => new FeedbackViewModel
                {
                    FeedbackID = f.FeedbackID,
                    QueryID = f.QueryID,
                    StudentID = f.StudentID,
                    Rating = f.Rating,
                    Comments = f.Comments,
                    SubmissionDate = f.SubmissionDate,
                    IsAnonymous = f.IsAnonymous
                })
                .ToListAsync();

            // If no feedback is found, return a message
            if (!studentFeedback.Any())
            {
                return NotFound("No feedback found.");
            }

            // Pass the feedback to the view
            return View("~/Views/Query/StaffQuery/ViewStudentFeedback.cshtml", studentFeedback);
        }




        // Action to render the dashboard view
        public IActionResult TicketDashboard()
        {
            return View("~/Views/Query/StaffQuery/TicketDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult GetQueriesForStaff(int staffId)
        {
            var queries = _context.QueryAssignments.Where(q => q.StaffID == staffId).ToList();
            return Json(queries);
        }

  


    }

}
