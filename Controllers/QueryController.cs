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
using Apptivate_UQMS_WebApp.Services.QueryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System;
using Apptivate_UQMS_WebApp.Controllers;

using NuGet.ContentModel;

namespace Apptivate_UQMS_WebApp.Controllers
{
  

    [TestClass]
    public class QueryControllerTests
    {
        private Mock<IQueryService> _mockQueryService;
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<ILogger<QueryController>> _mockLogger;
        private Mock<IEmailService> _mockEmailService;
        private QueryController _controller;
        private DefaultHttpContext _httpContext;

        [TestInitialize]
        public void Setup()
        {
            // Create mock dependencies
            _mockQueryService = new Mock<IQueryService>();
            _mockContext = new Mock<ApplicationDbContext>();
            _mockLogger = new Mock<ILogger<QueryController>>();
            _mockEmailService = new Mock<IEmailService>();

            // Create the controller
            _controller = new QueryController(
                _mockQueryService.Object,
                _mockContext.Object,
                _mockLogger.Object,
                _mockEmailService.Object
            );

            // Setup HTTP context with session
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [TestMethod]
        public async Task CreateQuery_ReturnsCorrectView()
        {
            // Act
            var result = await _controller.CreateQuery();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual("StudentQuery/NewQuery/CreateQuery", viewResult.ViewName);
        }

        [TestMethod]
        public async Task AcademicQuery_UserNotLoggedIn_RedirectsToLogin()
        {
            // Arrange
            // No Firebase UID in session simulates not being logged in

            // Act
            var result = await _controller.AcademicQuery(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("Login", redirectResult.ActionName);
            Assert.AreEqual("Account", redirectResult.ControllerName);
        }

        [TestMethod]
        public async Task AcademicQuery_ValidRequest_ReturnsView()
        {
            // Arrange
            // Set up a mock Firebase UID in session
            _httpContext.Session.SetString("FirebaseUID", "test-uid");

            // Mock the query service to return a dynamic object
            var mockQueryDetails = new
            {
                QueryTypeID = 1,
                QueryCategories = new object(),
                StudentDetail = new
                {
                    StudentID = "S001",
                    Department = "Computer Science",
                    DepartmentID = 1,
                    CourseCode = "CS101",
                    CourseID = 1,
                    Year = 2024
                }
            };

            _mockQueryService
                .Setup(s => s.GetAcademicQueryAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(mockQueryDetails);

            // Act
            var result = await _controller.AcademicQuery(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual("StudentQuery/NewQuery/AcademicQuery", viewResult.ViewName);
        }

        [TestMethod]
        public async Task SubmitAcademicQuery_ValidModel_SuccessfulSubmission()
        {
            // Arrange
            // Set up a mock Firebase UID in session
            _httpContext.Session.SetString("FirebaseUID", "test-uid");

            var queryDto = new QueryDto
            {
                Description = "Test query description"
            };

            // Mock the query service submission method
            _mockQueryService
                .Setup(s => s.SubmitAcademicQueryAsync(It.IsAny<QueryDto>(), It.IsAny<IFormFile>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SubmitAcademicQuery(queryDto, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            var redirectResult = result as RedirectToActionResult;
            Assert.AreEqual("CreateQuery", redirectResult.ActionName);
        }

        [TestMethod]
        public async Task SubmitAcademicQuery_InvalidDescription_ReturnsView()
        {
            // Arrange
            // Set up a mock Firebase UID in session
            _httpContext.Session.SetString("FirebaseUID", "test-uid");

            var queryDto = new QueryDto
            {
                Description = new string('A', 151) // Description over 150 characters
            };

            // Act
            var result = await _controller.SubmitAcademicQuery(queryDto, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var viewResult = result as ViewResult;
            Assert.AreEqual("StudentQuery/NewQuery/AcademicQuery", viewResult.ViewName);
        }
    }
}