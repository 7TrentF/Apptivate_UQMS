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

            // Fetch the IDs dynamically from the database
            var academicQueryType = await _context.QueryTypes.FirstOrDefaultAsync(qt => qt.TypeName == "Academic");
            var gradeRemarksCategory = await _context.QueryCategories.FirstOrDefaultAsync(qc => qc.CategoryName == "Grade Remarks" && qc.QueryTypeID == academicQueryType.QueryTypeID);
            var courseRegistrationCategory = await _context.QueryCategories.FirstOrDefaultAsync(qc => qc.CategoryName == "Course Registration" && qc.QueryTypeID == academicQueryType.QueryTypeID);

            // Ensure query is categorized as Academic > Grade Remarks or Course Registration
            if (model.QueryTypeID != academicQueryType.QueryTypeID || (model.CategoryID != gradeRemarksCategory.CategoryID && model.CategoryID != courseRegistrationCategory.CategoryID))
            {
                _logger.LogError("Invalid query type or category.");
                throw new Exception("Invalid query type or category.");
            }


            // Create the query record
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

            _context.Queries.Add(query);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Query with ID {QueryID} successfully created.", query.QueryID);

            // Handle file upload if exists
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

            // Find lecturers in the same department and perform load balancing
            var departmentName = await _context.Departments
                .Where(d => d.DepartmentID == query.DepartmentID)
                .Select(d => d.DepartmentName)
                .FirstOrDefaultAsync();

            var lecturers = await _context.StaffDetails
                .Where(s => s.Department == departmentName && s.Position.PositionName == "Lecturer")
                .ToListAsync();

            if (lecturers.Any())
            {
                var leastBusyLecturer = lecturers
                    .OrderBy(s => _context.QueryAssignments.Count(qa => qa.StaffID == s.StaffID && qa.ResolutionDate == null))
                    .FirstOrDefault();

                if (leastBusyLecturer != null)
                {
                    var queryAssignment = new QueryAssignment
                    {
                        QueryID = query.QueryID,
                        StaffID = leastBusyLecturer.StaffID,
                        AssignedDate = DateTime.Now
                    };

                    _context.QueryAssignments.Add(queryAssignment);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Query assigned to Lecturer with StaffID {StaffID}.", leastBusyLecturer.StaffID);
                }
            }
            else
            {
                // Escalate to department head if no lecturer found
                var departmentHead = await _context.StaffDetails
                    .FirstOrDefaultAsync(s => s.Department == departmentName && s.Position.PositionName == "Department Head");

                if (departmentHead != null)
                {
                    var queryAssignment = new QueryAssignment
                    {
                        QueryID = query.QueryID,
                        StaffID = departmentHead.StaffID,
                        AssignedDate = DateTime.Now
                    };

                    _context.QueryAssignments.Add(queryAssignment);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Query escalated to Department Head with StaffID {StaffID}.", departmentHead.StaffID);
                }
                else
                {
                    _logger.LogWarning("No department head found for QueryID {QueryID}.", query.QueryID);
                }
            }

            _logger.LogInformation("Query submission process completed successfully for QueryID {QueryID}.", query.QueryID);
        }
    }


}

