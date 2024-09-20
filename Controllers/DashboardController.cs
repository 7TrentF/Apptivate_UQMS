using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.StudentDashboardViewModel;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize]  // All users must be authorized to access the dashboard
    public class DashboardController : Controller
    {

        private readonly ILogger<StaffQueryController> _logger;
        private readonly ApplicationDbContext _context; // Inject _context


        public DashboardController(ApplicationDbContext context, ILogger<StaffQueryController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Index()
        {
            // Check if the user is a Student and redirect to the Student Dashboard
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("StudentDashboard");
            }

            // Check if the user is a Staff member and redirect to the Staff Dashboard
            if (User.IsInRole("Staff"))
            {
                return RedirectToAction("StaffDashboard");
            }

            // Check if the user is an Admin and redirect to the Admin Dashboard
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }

            // Default fallback (if the user has no role, which shouldn't happen)
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> StudentDashboard()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (firebaseUid == null)
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Find the student based on the logged-in Firebase UID
            var student = await _context.StudentDetails
                                        .Include(s => s.User)
                                        .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (student == null)
            {
                _logger.LogError("Student not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                return NotFound("Student not found.");
            }

            // Efficiently fetch counts for each status
            var queryStatusCounts = await _context.Queries
                .Where(q => q.StudentID == student.StudentID)
                .GroupBy(q => q.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Extract counts from the query result
            var pendingCount = queryStatusCounts.FirstOrDefault(q => q.Status == "Pending")?.Count ?? 0;
            var resolvedCount = queryStatusCounts.FirstOrDefault(q => q.Status == "Resolved")?.Count ?? 0;
            var inProgressCount = queryStatusCounts.FirstOrDefault(q => q.Status == "In Progress")?.Count ?? 0;

            // Fetch the recent queries for the student
            var recentQueries = await _context.Queries
                .Where(q => q.StudentID == student.StudentID)
                .Include(q => q.QueryDocuments)
                .ToListAsync();

            // Create the combined view model
            var viewModel = new StudentDashboardViewModel
            {
                RecentQueries = recentQueries,
                DashboardStats = new DashboardStatsViewModel
                {
                    PendingCount = pendingCount,
                    ResolvedCount = resolvedCount,
                    InProgressCount = inProgressCount
                }
            };

            return View(viewModel);
        }






        [Authorize(Roles = "Staff")]
        public IActionResult StaffDashboard()
        {
            // Return the staff dashboard view
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            // Return the admin dashboard view
            return View();
        }
    }
}
