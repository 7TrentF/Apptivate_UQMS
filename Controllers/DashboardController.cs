using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize]  // All users must be authorized to access the dashboard
    public class DashboardController : Controller
    {
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
        public IActionResult StudentDashboard()
        {
            // Return the student dashboard view
            return View();
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
