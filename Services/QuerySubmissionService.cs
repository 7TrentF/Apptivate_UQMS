using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;


namespace Apptivate_UQMS_WebApp.Services
{
    public interface IQueryService
    {
        Task SubmitAcademicQueryAsync(Query model, IFormFile uploadedFile, string firebaseUid);
    }

    public class QuerySubmissionService : IQueryService
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;  // Inject fileUploadService
        private readonly ILogger<QuerySubmissionService> _logger;

        public QuerySubmissionService(ApplicationDbContext context, FileUploadService fileUploadService, ILogger<QuerySubmissionService> logger)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }
        public async Task SubmitAcademicQueryAsync(Query model, IFormFile uploadedFile, string firebaseUid)
        {
            _logger.LogInformation("Query submission process started for FirebaseUID: {FirebaseUID}", firebaseUid);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                _logger.LogError("User with FirebaseUID {FirebaseUID} not found.", firebaseUid);
                throw new Exception("User not found.");
            }

            _logger.LogInformation("User with FirebaseUID {FirebaseUID} found. UserID: {UserID}", firebaseUid, user.UserID);

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
                    DepartmentID = department != null ? department.DepartmentID : (int?)null,
                    CourseID = course != null ? course.CourseID : (int?)null,
                    student.Year
                }).FirstOrDefaultAsync();

            if (studentDetailQuery == null)
            {
                _logger.LogError("Student details not found for UserID {UserID}.", user.UserID);
                throw new Exception("Student details not found.");
            }

            _logger.LogInformation("Student details found. StudentID: {StudentID}, DepartmentID: {DepartmentID}, CourseID: {CourseID}, Year: {Year}",
                studentDetailQuery.StudentID, studentDetailQuery.DepartmentID, studentDetailQuery.CourseID, studentDetailQuery.Year);

            var query = new Query
            {
                StudentID = studentDetailQuery.StudentID,
                QueryTypeID = model.QueryTypeID,
                CategoryID = model.CategoryID,
                DepartmentID = studentDetailQuery.DepartmentID ?? 0,
                CourseID = studentDetailQuery.CourseID ?? 0,
                Year = studentDetailQuery.Year,
                Description = model.Description,
                Status = "Pending", // Set status to pending
                SubmissionDate = DateTime.Now
            };

            _logger.LogInformation("Creating new query with pending status for StudentID {StudentID}.", studentDetailQuery.StudentID);

            _context.Queries.Add(query);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Query with ID {QueryID} successfully created.", query.QueryID);

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".zip" };
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Unsupported file type: {FileExtension}.", fileExtension);
                    throw new Exception("Unsupported file type. Please upload a .jpg, .png, .pdf, or .zip file.");
                }

                try
                {
                    var documentUrl = await _fileUploadService.UploadFileAsync(uploadedFile);
                    _logger.LogInformation("File {FileName} uploaded successfully.", uploadedFile.FileName);

                    var queryDocument = new QueryDocument
                    {
                        QueryID = query.QueryID,
                        DocumentName = uploadedFile.FileName,
                        DocumentPath = documentUrl,
                        UploadDate = DateTime.Now
                    };

                    _context.QueryDocuments.Add(queryDocument);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Document associated with QueryID {QueryID}.", query.QueryID);
                }
                catch (Exception ex)
                {
                    _logger.LogError("File upload failed: {Message}", ex.Message);
                    throw new Exception("File upload failed. Please try again.");
                }
            }

            // Find staff members who are in the same department as the student (by comparing department name)
            var departmentName = await _context.Departments
                .Where(d => d.DepartmentID == query.DepartmentID)
                .Select(d => d.DepartmentName)
                .FirstOrDefaultAsync();

            var staffMembers = await _context.StaffDetails
                .Where(s => s.Department == departmentName && s.Position.PositionName == "Lecturer") // Match by department name and position
                .ToListAsync();

            if (staffMembers.Any())
            {
                var staffMember = staffMembers.First(); // You can improve this by adding load balancing logic if needed
                var queryAssignment = new QueryAssignment
                {
                    QueryID = query.QueryID,
                    StaffID = staffMember.StaffID,
                    AssignedDate = DateTime.Now
                };

                _context.QueryAssignments.Add(queryAssignment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Query assigned to StaffID {StaffID}.", staffMember.StaffID);
            }
            else
            {
                _logger.LogWarning("No 'Lecturer' staff found in the department for QueryID {QueryID}.", query.QueryID);
            }

            _logger.LogInformation("Query submission process completed successfully for QueryID {QueryID}.", query.QueryID);
        }


    }
}
