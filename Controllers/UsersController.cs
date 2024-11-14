using Apptivate_UQMS_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System;
using Apptivate_UQMS_WebApp.Data;
using FirebaseAdmin.Auth.Hash;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.AppUsers;
using static Apptivate_UQMS_WebApp.Models.AdminDashboardViewModel;
using Microsoft.CodeAnalysis.Scripting;
using Apptivate_UQMS_WebApp.Services.AccountServices;
using System.Security.Claims;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize(Roles = "Admin")] //only Admins can access these actions
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly FirebaseAuthService _firebaseAuthService;

        // Constructor injecting the database context
        public UsersController(FirebaseAuthService firebaseAuthService, ApplicationDbContext context)
        {
            _firebaseAuthService = firebaseAuthService;
           _context = context;
        }

        // GET: Users/UserManagement
        public async Task<IActionResult> UserManagement(string searchString, string sortOrder)
        {
            // Pass current sort state to the view
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["RoleSortParm"] = sortOrder == "Role" ? "role_desc" : "Role";

            // Fetch users from the database
            var users = from u in _context.Users
                        select u;

            // Searching
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Name.Contains(searchString)
                                       || u.Surname.Contains(searchString)
                                       || u.Email.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(u => u.Name);
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                case "Role":
                    users = users.OrderBy(u => u.Role);
                    break;
                case "role_desc":
                    users = users.OrderByDescending(u => u.Role);
                    break;
                default:
                    users = users.OrderBy(u => u.Name);
                    break;
            }

            var userList = await users.ToListAsync();


            // Get counts for each role
            int staffCount = userList.Count(u => u.Role == "Staff");
            int studentCount = userList.Count(u => u.Role == "Student");
            int adminCount = userList.Count(u => u.Role == "Admin");

            // Populate the ViewModel
            var viewModel = new AdminDashboardViewModel
            {
                // Stats
                TotalUsers = userList.Count,
                ActiveUsers = userList.Count(u => u.LastSeen > DateTime.Now.AddDays(-30)), // Example criteria
                TotalQueries = _context.Queries.Count(),
                ResolvedQueries = _context.Queries.Count(q => q.Status == QueryModel.QueryStatus.Resolved),
                PendingQueries = _context.Queries.Count(q => q.Status == QueryModel.QueryStatus.Pending),

                StaffCount = staffCount,
                StudentCount = studentCount,
                AdminCount = adminCount,

                Users = userList.FirstOrDefault(), // Assuming single admin user viewing

                // User Management Overview
                UserManagementOverview = _context.Users
                    .GroupBy(u => u.Role)
                    .Select(g => new UserManagementViewModel
                    {
                        Role = g.Key,
                        Count = g.Count(),
                        Percentage = (int)((g.Count() * 100.0) / userList.Count)
                    }).ToList(),


            };

            // Users List for the table
            viewModel.UsersList = userList;

            return View(viewModel);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Surname,Email,Password,Role")] ApplicationUsers user)
        {
            // Hash the password before saving (using BCrypt)
            var hashedPassword = _firebaseAuthService.GeneratePasswordHash(user.Password);
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    PasswordHash = hashedPassword,
                    Role = user.Role,
                    RegistrationDate = DateTime.Now
                };
                _context.Add(newUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserManagement));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserID,Name,Surname,Email,Role")] User user)
        {
            if (id != user.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserManagement));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserManagement));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserID == id);
        }


        [HttpGet("GetOnlineStatuses")]
        public IActionResult GetOnlineStatuses()
        {
            var statuses = _context.Users.Select(u => new { id = u.UserID, isOnline = u.IsOnline }).ToList();
            return Json(statuses);
        }
    }
}