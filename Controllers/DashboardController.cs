using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Models;
using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.StudentDashboardViewModel;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize]  // All users must be authorized to access the dashboard
    public class DashboardController : Controller
    {

        private readonly ILogger<StaffQueryController> _logger;
        private readonly ApplicationDbContext _context; // Inject _context
        private readonly IUserProfileService _userProfileService;

        public DashboardController(ApplicationDbContext context, ILogger<StaffQueryController> logger,IUserProfileService userProfileService)
        {
            _context = context;
            _logger = logger;
            _userProfileService = userProfileService;
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


            //fetch the Details for the student
            var userProfile = await _userProfileService.GetUserProfileAsync(firebaseUid);
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }


         // Create the combined view model
         var viewModel = new StudentDashboardViewModel
            {
                RecentQueries = recentQueries,
                DashboardStats = new DashboardStatsViewModel
                {
                    PendingCount = pendingCount,
                    ResolvedCount = resolvedCount,
                    InProgressCount = inProgressCount
                },

                StudentDetails = userProfile.StudentDetail,
                Users = userProfile.User
               

            };

            return View(viewModel);
        }




        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> StaffDashboard()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Find the staff member based on the logged-in Firebase UID
            var staff = await _context.StaffDetails
                                      .Include(s => s.User)
                                      .Include(s => s.Position)
                                      .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (staff == null)
            {
                _logger.LogError("Staff not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                return NotFound("Staff not found.");
            }

            // Fetch counts for each query status
            var queryStatusCounts = await _context.Queries
                .Where(q => q.QueryAssignments.Any(qa => qa.StaffID == staff.StaffID))
                .GroupBy(q => q.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var pendingCount = queryStatusCounts.FirstOrDefault(q => q.Status == "Pending")?.Count ?? 0;
            var resolvedCount = queryStatusCounts.FirstOrDefault(q => q.Status == "Resolved")?.Count ?? 0;
            var inProgressCount = queryStatusCounts.FirstOrDefault(q => q.Status == "In Progress")?.Count ?? 0;

            // Fetch the queries assigned to this staff member with related data
            var queryAssignments = await _context.QueryAssignments
                .Where(qa => qa.StaffID == staff.StaffID)
                .Include(qa => qa.Query)
                    .ThenInclude(q => q.Student)
                        .ThenInclude(s => s.User) // To get Student's Email
                .Include(qa => qa.Query)
                    .ThenInclude(q => q.Category)
                        .ThenInclude(c => c.QueryType)
                .Include(qa => qa.Query)
                    .ThenInclude(q => q.Department)
                .ToListAsync();

            // Fetch Team Overview Data
            var teamMembers = await _context.StaffDetails
                .Where(s => s.QueryAssignments.Any(qa => qa.Query != null))
                .Include(s => s.User)
                .Include(s => s.Position)
                .Include(s => s.QueryAssignments)
                    .ThenInclude(qa => qa.Query)
                        .ThenInclude(q => q.Category)
                            .ThenInclude(c => c.QueryType)
                .ToListAsync();

            var teamOverview = teamMembers.Select(s => new TeamMemberViewModel
            {
                StaffID = s.StaffID,
                StaffName = $"{s.User.Name} {s.User.Surname}",
                Position = s.Position?.PositionName ?? "N/A",
                Department = s.Department,
                QueryTypes = s.QueryAssignments
                                .Select(qa => qa.Query?.Category?.QueryType?.TypeName)
                                .Where(qt => !string.IsNullOrEmpty(qt))
                                .Distinct()
                                .ToList()
            }).ToList();

            // Fetch the Details for the staff
            var userProfile = await _userProfileService.GetUserProfileAsync(firebaseUid);
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }

            // Create the combined view model
            var viewModel = new StaffDashboardViewModel
            {
                AssignedQueries = queryAssignments,
                DashboardStats = new DashboardStatsViewModel
                {
                    PendingCount = pendingCount,
                    ResolvedCount = resolvedCount,
                    InProgressCount = inProgressCount
                },
                StaffDetails = userProfile.StaffDetail,
                Users = userProfile.User,
                TeamOverview = teamOverview // Populate Team Overview
            };

            return View(viewModel);
        }




        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var firebaseUid = HttpContext.Session.GetString("FirebaseUID");

            if (string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogError("User not logged in.");
                return RedirectToAction("Login", "Account");
            }

            // Find the staff member based on the logged-in Firebase UID
            var admin = await _context.AdminDetails
                                      .Include(s => s.User)
                                      .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (admin == null)
            {
                _logger.LogError("Staff not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                return NotFound("Staff not found.");
            }

            // Fetch the Details for the staff
            var userProfile = await _userProfileService.GetUserProfileAsync(firebaseUid);
            if (userProfile == null)
            {
                return NotFound("User not found.");
            }


            // Fetch total users
            var totalUsers = await _context.Users.CountAsync();

            /*
            // Fetch active users (assuming 'Active' status is determined by some property, e.g., last login date)
            var activeUsers = await _context.Users
                .Where(u => u.LastLoginDate >= DateTime.UtcNow.AddMonths(-1)) // Example condition
                .CountAsync(); */

            // Fetch total queries
            var totalQueries = await _context.Queries.CountAsync();

            // Fetch resolved queries
            var resolvedQueries = await _context.Queries
                .Where(q => q.Status == "Resolved")
                .CountAsync();

            // Fetch pending queries
            var pendingQueries = await _context.Queries
                .Where(q => q.Status == "Pending")
                .CountAsync();

            // User Management Overview
            var userManagementOverview = await _context.Users
                .GroupBy(u => u.Role)
                .Select(g => new UserManagementViewModel
                {
                    Role = g.Key,
                    Count = g.Count(),
                    Percentage = (int)(g.Count() * 100.0 / totalUsers)
                })
                .ToListAsync();

            // Weekly Activity Data
            var openQueries = await _context.Queries
                .Where(q => q.Status == "Open")
                .GroupBy(q => q.SubmissionDate.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => g.Count())
                .ToListAsync();

            var reQueries = await _context.Queries
                .Where(q => q.Status == "Reopened")
                .GroupBy(q => q.SubmissionDate.Value.Date)
                .OrderBy(g => g.Key)
                .Select(g => g.Count())
                .ToListAsync();

            // Card Expense Statistics Data
            // Assuming you have a way to fetch this data; placeholder example
            var cardExpenseData = new List<int> { 300, 200, 150, 100 };

            // Queries Received Data
            var queriesReceivedData = await _context.Queries
                .GroupBy(q => q.SubmissionDate.Value.Month)
                .OrderBy(g => g.Key)
                .Select(g => g.Count())
                .ToListAsync();

            // System Activity - Fetch recent activities
            var systemActivities = await _context.Queries
                .Include(q => q.Student)
                .ThenInclude(s => s.User)
                .OrderByDescending(q => q.SubmissionDate)
                .Take(5)
                .Select(q => new SystemActivityViewModel
                {
                    UserName = q.Student != null ? $"{q.Student.User.Name} {q.Student.User.Surname}" : "Unknown",
                    Action = $"Submitted a query: {q.Description}",
                    Timestamp = q.SubmissionDate.HasValue ? q.SubmissionDate.Value.ToString("dd MMM yyyy, h:mm tt") : "N/A"
                })
                .ToListAsync();

            // Populate ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                TotalUsers = totalUsers,
                //ActiveUsers = activeUsers,
                Users = userProfile.User,
                TotalQueries = totalQueries,
                ResolvedQueries = resolvedQueries,
                PendingQueries = pendingQueries,
                UserManagementOverview = userManagementOverview,
                OpenQueries = openQueries,
                ReQueries = reQueries,
                CardExpenseData = cardExpenseData,
                QueriesReceivedData = queriesReceivedData,
                SystemActivities = systemActivities
            };

            return View(viewModel);
        }
    }
    
}
