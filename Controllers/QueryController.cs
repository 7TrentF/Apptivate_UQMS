﻿using Apptivate_UQMS_WebApp.Data;
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
namespace Apptivate_UQMS_WebApp.Controllers
{

    [Authorize(Roles = "Student")]
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
           


            return View("StudentQuery/NewQuery/AcademicQuery");
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

            return View("StudentQuery/NewQuery/AdministrativeQuery");
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
                    return View("StudentQuery/NewQuery/AcademicQuery"); // Return the form if validation fails
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
            return View("StudentQuery/NewQuery/QuerySubmitted");
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
                    return View("StudentQuery/NewQuery/AcademicQuery"); // Return the form if validation fails
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
            return View("StudentQuery/NewQuery/QuerySubmitted");
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
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status == "New")
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
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status == "On-going")
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
                .Where(q => q.StudentID == studentDetail.StudentID && q.Status == "Resolved")
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

            if (!string.IsNullOrEmpty(statusFilter))
            {
                queryResults = queryResults.Where(q => q.Status == statusFilter);
            }

            queryResults = queryResults.Where(q => q.StudentID == studentDetail.StudentID);

            var queries = await queryResults.ToListAsync();

            return PartialView("StudentQuery/QueryOverview/AllTickets", queries);
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
