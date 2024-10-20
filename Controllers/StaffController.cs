﻿using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.QueryModel.QueryResolutions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Apptivate_UQMS_WebApp.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffQueryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StaffQueryController> _logger;
        private readonly IHubContext<NotificationHub> _hubContext;  // Inject SignalR Hub Context
        private readonly IQueryService _queryService;  // Inject IQueryService

        public StaffQueryController(IQueryService queryService, ApplicationDbContext context, ILogger<StaffQueryController> logger, IHubContext<NotificationHub> hubContext)
        {
            _queryService = queryService;

            _context = context;
            _logger = logger;
            _hubContext = hubContext;
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

        // Action to mark a query as resolved
        [HttpPost]
        public async Task<IActionResult> MarkAsResolved(int queryId)
        {
            var queryAssignment = await _context.QueryAssignments
                                                .FirstOrDefaultAsync(qa => qa.QueryID == queryId);

            if (queryAssignment == null)
            {
                _logger.LogError("QueryAssignment not found for QueryID {QueryID}.", queryId);
                return NotFound();
            }

            queryAssignment.ResolutionDate = DateTime.Now;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Query with ID {QueryID} marked as resolved by StaffID {StaffID}.", queryId, queryAssignment.StaffID);
            return RedirectToAction("MyQueries");
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



        /*
        [HttpPost]
        public async Task<IActionResult> SubmitSolutionToQueryAsync(QueryResolutions model, IFormFile uploadedFile)
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
                if (model.Solution.Length > 150)
                {
                    ModelState.AddModelError("Solution", "Solution cannot be longer than 150 characters.");
                    return View("~/Views/Query/StaffQuery/QueryDetails.cshtml"); // Load the view
                }

                try
                {
                    await _queryService.SubmitSolutionToQueryAsync(model, uploadedFile, firebaseUid);

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
        //Create solution to query 
        */


        public async Task<object> GetAcademicQueryAsync(int queryTypeId, string firebaseUid)
        {
            _logger.LogInformation($"GetAcademicQuery called with queryTypeId: {queryTypeId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }

            // Updated query to retrieve CourseID and DepartmentID
            var studentDetailQuery = await (
                from student in _context.StudentDetails
                join department in _context.Departments
                on student.Department equals department.DepartmentName into deptGroup
                from department in deptGroup.DefaultIfEmpty()
                join course in _context.Courses
                on student.Course equals course.CourseCode into courseGroup
                from course in courseGroup.DefaultIfEmpty()
                where student.UserID == user.UserID
                select new
                {
                    student.StudentID,
                    CourseCode = course.CourseCode,
                    Department = department.DepartmentName,
                    DepartmentID = department != null ? department.DepartmentID : (int?)null,
                    CourseID = course != null ? course.CourseID : (int?)null,
                    student.Year
                }).FirstOrDefaultAsync();

            if (studentDetailQuery == null)
            {
                _logger.LogError("Student details not found.");
                throw new Exception("Student details not found.");
            }

            var queryType = await _context.QueryTypes
                                          .Include(qt => qt.QueryCategories)
                                          .FirstOrDefaultAsync(qt => qt.QueryTypeID == queryTypeId);
            if (queryType == null)
            {
                _logger.LogError("Query type not found.");
                throw new Exception("Query type not found.");
            }

            // Map QueryType to DTO
            var queryTypeDto = new QueryTypeDto
            {
                QueryTypeID = queryType.QueryTypeID,
                QueryCategories = queryType.QueryCategories.Select(qc => new QueryCategoryDto
                {
                    CategoryID = qc.CategoryID,
                    CategoryName = qc.CategoryName
                }).ToList()
            };

            // Return CourseID and DepartmentID along with other details
            return new
            {
                QueryTypeID = queryTypeId,
                QueryCategories = queryTypeDto.QueryCategories,
                StudentDetail = new
                {
                    studentDetailQuery.CourseCode,

                    studentDetailQuery.Department,
                    studentDetailQuery.StudentID,
                    studentDetailQuery.DepartmentID,  // Return DepartmentID
                    studentDetailQuery.CourseID,      // Return CourseID
                    studentDetailQuery.Year
                }
            };
        }





    }

}
