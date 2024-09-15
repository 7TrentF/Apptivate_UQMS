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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null)
            {
                _logger.LogError("User with FirebaseUID {FirebaseUID} not found.", firebaseUid);
                throw new Exception("User not found.");
            }


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

            var query = new Query
            {
                StudentID = studentDetailQuery.StudentID,
                QueryTypeID = model.QueryTypeID,
                CategoryID = model.CategoryID,
                DepartmentID = studentDetailQuery.DepartmentID ?? 0,
                CourseID = studentDetailQuery.CourseID ?? 0,
                Year = studentDetailQuery.Year,
                Description = model.Description, 
                Status = "Pending",
                SubmissionDate = DateTime.Now
            };

            _context.Queries.Add(query);
            await _context.SaveChangesAsync();

            if (uploadedFile != null && uploadedFile.Length > 0)
            {
                // Check file type
                var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".zip" };
                var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning("Unsupported file type: {FileExtension}", fileExtension);
                    throw new Exception("Unsupported file type. Please upload a .jpg, .png, .pdf, or .zip file.");
                }

                try
                {
                    var documentUrl = await _fileUploadService.UploadFileAsync(uploadedFile);

                    var queryDocument = new QueryDocument
                    {
                        QueryID = query.QueryID,
                        DocumentName = uploadedFile.FileName,
                        DocumentPath = documentUrl,
                        UploadDate = DateTime.Now
                    };

                    _context.QueryDocuments.Add(queryDocument);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("File upload failed: {Message}", ex.Message);
                    throw new Exception("File upload failed. Please try again.");
                }
            }

            _logger.LogInformation("Query with ID {QueryID} successfully submitted.", query.QueryID);
        }
    }

}
