using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using QueryStatus = Apptivate_UQMS_WebApp.Models.QueryModel.QueryStatus;
using Query = Apptivate_UQMS_WebApp.Models.QueryModel.Query;
using Apptivate_UQMS_WebApp.ViewModels;
using static Apptivate_UQMS_WebApp.ViewModels.QueryViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Apptivate_UQMS_WebApp.Hubs;


namespace Apptivate_UQMS_WebApp.Services.QueryServices
{
    public class QuerySubmissionService : IQueryService
    {
        private readonly ApplicationDbContext _context;
        private readonly FileUploadService _fileUploadService;  // Inject fileUploadService
        private readonly ILogger<QuerySubmissionService> _logger;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public QuerySubmissionService(IEmailService emailService, ApplicationDbContext context, FileUploadService fileUploadService, ILogger<QuerySubmissionService> logger, INotificationService notificationService)
        {
            _context = context; 
            _fileUploadService = fileUploadService;
            _emailService = emailService; 
            _notificationService = notificationService;
            _logger = logger;
        }

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
                    course.CourseCode,
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
                queryTypeDto.QueryCategories,
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

        public string GenerateDateBasedTicketNumber()
        {
            var random = new Random();
            string datePart = DateTime.UtcNow.ToString("yyyyMMdd");  // Example: 20241022
            string randomPart = random.Next(10000, 99999).ToString(); // Example: 58423
            return $"{datePart}-{randomPart}";
        }

        public async Task SubmitAcademicQueryAsync(QueryDto model, IFormFile uploadedFile, string firebaseUid)
        {
            _logger.LogInformation("Query submission process started for FirebaseUID: {FirebaseUID}", firebaseUid);

            var queryTypeID = model.QueryTypeID;

            try
            {
                // Use the existing method to fetch the student details and query type
                var academicQueryDetails = await GetAcademicQueryAsync(queryTypeID, firebaseUid);

                // Extract the necessary details
                dynamic queryData = academicQueryDetails;
                var studentDetail = queryData.StudentDetail;
                var queryCategories = queryData.QueryCategories;

                _logger.LogInformation("Received QueryTypeID: {QueryTypeID}, CategoryID: {CategoryID}", model.QueryTypeID, model.CategoryID);
                _logger.LogInformation("Student details:" + model.StudentID, model.QueryTypeID, model.DepartmentID, model.Year);

                // Validate the query type and category
                if (model.QueryTypeID != queryData.QueryTypeID)
                {
                    _logger.LogError("Invalid query type or category.");
                    throw new Exception("Invalid query type or category.");
                }

                // Create the query record
                var query = new Query
                {
                    StudentID = studentDetail.StudentID,
                    QueryTypeID = model.QueryTypeID,
                    TicketNumber = GenerateDateBasedTicketNumber(), // or GenerateDateBasedTicketNumber()
                    CategoryID = model.CategoryID,
                    DepartmentID = studentDetail.DepartmentID ?? 0,
                    CourseID = studentDetail.CourseID ?? 0,
                    Year = studentDetail.Year,
                    Description = model.Description,
                    Status = QueryStatus.Pending, // Update status to Ongoing
                    SubmissionDate = DateTime.Now
                };

                _context.Queries.Add(query);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Query with ID {QueryID} successfully created.", query.QueryID);

                // Handle file upload if exists
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    await HandleFileUpload(uploadedFile, query.QueryID);
                }

                // Assign query to the least busy lecturer or escalate
                await AssignQueryToStaffAsync(query);

                _logger.LogInformation("Query submission process completed successfully for QueryID {QueryID}.", query.QueryID);
                await SendQuerySubmissionNotificationEmailAsync(query, model, firebaseUid);
            }

            catch (Exception ex)
            {
                _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                throw new Exception("Query submission failed.");
            }
        }

        private async Task SendQuerySubmissionNotificationEmailAsync(Query query, QueryDto model, string firebaseUid)
        {
            _logger.LogInformation($"GetStudentEmailAsync called with firebaseUid: {firebaseUid}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }

            // Fetch the user's email based on the Firebase UID
            var studentEmail = await _context.Users
                .Where(u => u.FirebaseUID == firebaseUid) // Replace with the actual column name for Firebase UID
                .Select(u => u.Email)
                .FirstOrDefaultAsync();


            if (studentEmail == null)
            {
                _logger.LogError("Student email not found.");
                throw new Exception("Student email  details not found.");
            }

            _logger.LogInformation("This is the email you require {UserEmail}", studentEmail);


            try
            {
                var emailData = new Dictionary<string, object>
        {
            { "user_name", studentEmail.Split('@')[0] },
            { "query_description", model.Description.Substring(0, Math.Min(50, model.Description.Length)) },
            { "ticket_number", query.TicketNumber },
            { "submitted_date", query.SubmissionDate?.ToString("f") }
        };

                // Replace '1' with your actual email template ID
                await _emailService.SendTemplateEmailAsync(studentEmail, 1, emailData);
                _logger.LogInformation("Notification email sent to user {UserEmail}", studentEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send query submission notification email to {UserEmail}: {Message}", studentEmail, ex.Message);
                throw new ApplicationException("Failed to send notification email", ex);
            }
        }

        private async Task SendQuerySubmissionNotificationEmailStaff(Query query,int staffId)
        {
            _logger.LogInformation($"Get Staff email called with staffId: {staffId}");


            // Fetch the user's email based on the Firebase UID
            var userId = await _context.StaffDetails
                 .Where(u => u.StaffID == staffId) // Replace with the actual column name for Firebase UID
                .Select(u => u.UserID)
                .FirstOrDefaultAsync();


            if (userId == null)
            {
                _logger.LogError("UserId not found.");
                throw new Exception("UserId not found.");
            }


            var staffEmail = await _context.Users
                .Where(u => u.UserID == userId) // Replace with the actual column name for Firebase UID
                .Select(u => u.Email)
                .FirstOrDefaultAsync();


            if (staffEmail == null)
            {
                _logger.LogError("Staff email not found.");
                throw new Exception("Staff email  details not found.");
            }

            _logger.LogInformation("This is the email you require {UserEmail}", staffEmail);


            try
            {
                var emailData = new Dictionary<string, object>
        {
            { "user_name", staffEmail.Split('@')[0] },
            { "query_description", query.Description.Substring(0, Math.Min(50, query.Description.Length)) },
            { "ticket_number", query.TicketNumber },
            { "submitted_date", query.SubmissionDate?.ToString("f") }
        };

                // Replace '1' with your actual email template ID
                await _emailService.SendTemplateEmailAsync(staffEmail, 2, emailData);
                _logger.LogInformation("Notification email sent to user {UserEmail}", staffEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send query submission notification email to {UserEmail}: {Message}", staffEmail, ex.Message);
                throw new ApplicationException("Failed to send notification email", ex);
            }
        }

       
        public async Task SubmitAdministrativeQueryAsync(QueryDto model, IFormFile uploadedFile, string firebaseUid)
        {
            _logger.LogInformation("Query submission process started for FirebaseUID: {FirebaseUID}", firebaseUid);

            var queryTypeID = model.QueryTypeID;

            try
            {

                // Use the existing method to fetch the student details and query type
                var academicQueryDetails = await GetAcademicQueryAsync(queryTypeID, firebaseUid);

                // Extract the necessary details
                dynamic queryData = academicQueryDetails;
                var studentDetail = queryData.StudentDetail;
                var queryCategories = queryData.QueryCategories;


                _logger.LogWarning("Received QueryTypeID: {QueryTypeID}, CategoryID: {CategoryID}", model.QueryTypeID, model.CategoryID);
                _logger.LogWarning("Student details:" + model.StudentID, model.QueryTypeID, model.DepartmentID, model.Year);


                // Validate the query type and category
                if (model.QueryTypeID != queryData.QueryTypeID)
                {
                    _logger.LogError("Invalid query type or category.");
                    throw new Exception("Invalid query type or category.");
                }

                // Create the query record
                var query = new Query
                {
                    StudentID = studentDetail.StudentID,
                    TicketNumber = GenerateDateBasedTicketNumber(), // or GenerateDateBasedTicketNumber()
                    QueryTypeID = model.QueryTypeID,
                    CategoryID = model.CategoryID,
                    DepartmentID = studentDetail.DepartmentID ?? 0,
                    CourseID = studentDetail.CourseID ?? 0,
                    Year = studentDetail.Year,
                    Description = model.Description,
                    Status = QueryStatus.Pending, // Update status to Ongoing
                    SubmissionDate = DateTime.Now
                };

                _context.Queries.Add(query);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Query with ID {QueryID} successfully created.", query.QueryID);

                // Handle file upload if exists
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    await HandleFileUpload(uploadedFile, query.QueryID);
                }

                // Assign query to the least busy lecturer or escalate
                await AssignQueryToStaffAsync(query);

                _logger.LogInformation("Query submission process completed successfully for QueryID {QueryID}.", query.QueryID);

                await SendQuerySubmissionNotificationEmailAsync(query, model, firebaseUid);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                throw new Exception("Query submission failed.");
            }
        }

        private async Task HandleFileUpload(IFormFile uploadedFile, int queryId)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".zip" };
            var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning("Unsupported file type: {FileExtension}.", fileExtension);
                throw new Exception("Unsupported file type. Please upload a .jpg, .png, .pdf, or .zip file.");
            }

            var documentUrl = await _fileUploadService.UploadFileAsync(uploadedFile);
            var queryDocument = new QueryDocument
            {
                QueryID = queryId,
                DocumentName = uploadedFile.FileName,
                DocumentPath = documentUrl,
                UploadDate = DateTime.Now
            };




            _context.QueryDocuments.Add(queryDocument);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Document associated with QueryID {QueryID}.", queryId);
        }

        private async Task AssignQueryToStaffAsync(Query query)
        {
            // Find the department name
            var departmentName = await _context.Departments
                .Where(d => d.DepartmentID == query.DepartmentID)
                .Select(d => d.DepartmentName)
                .FirstOrDefaultAsync();

            // Find lecturers in the same department
            var lecturers = await _context.StaffDetails
                .Where(s => s.Department == departmentName && s.Position.PositionName == "Lecturer")
                .ToListAsync();

            if (lecturers.Any())
            {
                // Find the least busy lecturer (based on unresolved queries)
                var leastBusyLecturer = lecturers
                    .OrderBy(s => _context.QueryAssignments.Count(qa => qa.StaffID == s.StaffID && qa.ResolutionDate == null))
                    .FirstOrDefault();

                if (leastBusyLecturer != null)
                {
                    // Assign the query to the least busy lecturer
                    var queryAssignment = new QueryAssignment
                    {
                        AssignmentNumber = GenerateDateBasedTicketNumber(),
                        QueryID = query.QueryID,
                        StaffID = leastBusyLecturer.StaffID,
                        AssignedDate = DateTime.Now
                    };

                    _context.QueryAssignments.Add(queryAssignment);
                    query.Status = QueryStatus.Ongoing; // Update status to Ongoing

                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Query assigned to Lecturer with StaffID {StaffID}.", leastBusyLecturer.StaffID);


                    await SendQuerySubmissionNotificationEmailStaff(query, leastBusyLecturer.StaffID);

                    _logger.LogInformation("sending email to  { leastBusyLecturer.StaffID}.", leastBusyLecturer.StaffID);

                    // Send notification to staff member
                    await _notificationService.NotifyStaffMember(leastBusyLecturer.StaffID, $"Query #{query.TicketNumber} has been assigned to you");
                    _logger.LogInformation("sent notification to  {StaffID}.", leastBusyLecturer.StaffID);



                }
            }
            else
            {
                // Escalate to department head if no lecturer is found
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
        }

        public async Task<object> GetStaffAssignmentQueryDetails(int queryId, string firebaseUid)
        {

            _logger.LogInformation($"GetStaffAssignmentQueryDetails called with queryTypeId: {queryId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }



            // Find the staff member based on the logged-in Firebase UID
            var staff = await _context.StaffDetails
                                      .Include(s => s.User)
                                      .FirstOrDefaultAsync(s => s.User.FirebaseUID == firebaseUid);

            if (staff == null)
            {
                _logger.LogError("Staff not found for FirebaseUID {FirebaseUID}.", firebaseUid);
                throw new Exception("Staff details not found.");
            }

            // Assume the specific QueryID is provided

            // Fetch the QueryAssignment for this staff member and QueryID
            var queryAssignment = await _context.QueryAssignments
                                      .Where(qa => qa.StaffID == staff.StaffID && qa.QueryID == queryId)
                                      .Select(qa => qa.AssignmentID)  // Select the AssignmentID
                                      .FirstOrDefaultAsync();

            if (queryAssignment == null)
            {
                _logger.LogError("No assignment found for StaffID {StaffID} and QueryID {QueryID}.", staff.StaffID, queryId);
                throw new Exception("Assignment not found.");
            }


            _logger.LogWarning("AssignmentID: " + queryAssignment);
            _logger.LogWarning("QueryID: " + queryId);

            _logger.LogInformation("Success !!!");

            // Return the AssignmentID for the specific QueryID
            return new
            {
                AssignmentID = queryAssignment,
                QueryID = queryId,

            };

        }

        public async Task<object> GetEmailAsync(string firebaseUid)
        {
            _logger.LogInformation($"GetStudentEmailAsync called with firebaseUid: {firebaseUid}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }

            // Fetch the user's email based on the Firebase UID
            var studentEmail = await _context.Users
                .Where(u => u.FirebaseUID == firebaseUid) // Replace with the actual column name for Firebase UID
                .Select(u => u.Email)
                .FirstOrDefaultAsync();


            if (studentEmail == null)
            {
                _logger.LogError("Student email not found.");
                throw new Exception("Student email  details not found.");
            }

            // Return the AssignmentID for the specific QueryID
            return new
            {
                Email = studentEmail,


            };




        }


        public async Task<object> GetStudentQueryAsync(int queryId, string firebaseUid)
        {
            _logger.LogInformation($"GetStudentQueryAsync called with queryId: {queryId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }

            // Fetch query, student, and user details
            var StudentQueryDetails = await _context.Queries
                .Include(q => q.QueryDocuments) // Include documents
                .Include(q => q.Student)        // Include student details
                .ThenInclude(s => s.User)   // Include associated user details for the student
                .Include(q => q.Department)     // Include department
                .Include(q => q.Course)         // Include course
                .FirstOrDefaultAsync(q => q.QueryID == queryId);

            if (StudentQueryDetails == null)
            {
                _logger.LogError("Student Query details not found.");
                throw new Exception("Student Query details not found.");
            }

            return StudentQueryDetails;
        }

        private async Task HandleFileUploadForResolution(IFormFile uploadedFile, int resolutionId, int queryId)
        {
            var allowedExtensions = new[] { ".jpg", ".png", ".pdf", ".zip" };
            var fileExtension = Path.GetExtension(uploadedFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning("Unsupported file type: {FileExtension}.", fileExtension);
                throw new Exception("Unsupported file type. Please upload a .jpg, .png, .pdf, or .zip file.");
            }

            // Upload file to Firebase Storage and get the URL
            var documentUrl = await _fileUploadService.UploadFileAsync(uploadedFile);

            // Insert the document metadata into QueryDocuments
            var queryDocument = new QueryDocument
            {
                QueryID = queryId,
                DocumentName = uploadedFile.FileName,
                DocumentPath = documentUrl,
                UploadDate = DateTime.Now
            };

            _context.QueryDocuments.Add(queryDocument);
            await _context.SaveChangesAsync();  // Save QueryDocuments to generate the DocumentID

            _logger.LogInformation("Document associated with QueryID {QueryID}.", queryId);

            // Insert into ResolutionDocuments to link the document to the resolution
            var resolutionDocument = new ResolutionDocuments
            {
                ResolutionID = resolutionId,
                DocumentID = queryDocument.DocumentID
            };

            _context.ResolutionDocuments.Add(resolutionDocument);
            await _context.SaveChangesAsync(); // Save ResolutionDocuments

            _logger.LogInformation("Document associated with ResolutionID {ResolutionID} and DocumentID {DocumentID}.", resolutionId, queryDocument.DocumentID);
        }

        public async Task SubmitSolutionToQueryAsync(QueryResolutions model, IFormFile uploadedFile, string firebaseUid)
        {
            _logger.LogInformation("Query resolution process started for FirebaseUID: {FirebaseUID}", firebaseUid);

            var queryID = model.QueryID;

            try
            {
                // Use the existing method to fetch the staff details
                var StaffQueryAssignment = await GetStaffAssignmentQueryDetails(queryID, firebaseUid);

                // Extract the necessary details
                dynamic queryData = StaffQueryAssignment;
                //   

                _logger.LogWarning("Received QueryID: {QueryID}, AssignmentID: {AssignmentID}", model.QueryID, model.AssignmentID);


                // Create the query record
                var queryResolution = new QueryResolutions
                {
                    AssignmentID = model.AssignmentID,
                    QueryID = model.QueryID,
                    Solution = model.Solution,
                    ApprovalStatus = model.ApprovalStatus,
                    AdditionalNotes = model.AdditionalNotes,
                    ResolutionDate = DateTime.Now
                };
                _context.QueryResolutions.Add(queryResolution);

                var query = await _context.Queries.FirstOrDefaultAsync(q => q.QueryID == model.QueryID);

                if (query != null)
                {
                    query.Status = QueryStatus.Resolved; // Update status to Resolved
                    query.ResolvedDate = DateTime.Now;
                }

                else
                {
                    _logger.LogError("Query with ID {QueryID} not found.", model.QueryID);
                    throw new Exception($"Query with ID {model.QueryID} not found.");
                }


                query.Status = QueryStatus.Resolved; // Update status to Resolved

                await _context.SaveChangesAsync(); // Save QueryResolutions to generate the ResolutionID
                _logger.LogInformation("Query with ResolutionID {ResolutionID} successfully created.", queryResolution.ResolutionID);

                // Handle file upload if a file exists
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    await HandleFileUploadForResolution(uploadedFile, queryResolution.ResolutionID, queryResolution.QueryID);
                }

                _logger.LogInformation("Query submission process completed successfully for QueryID {QueryID}.", queryResolution.QueryID);

                //await ApprovalStatusAsync(queryResolution);

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while submitting the query: {Message}", ex.Message);
                throw new Exception("Query submission failed.");
            }
        }






        public async Task<ResolvedTicketAndQueryViewModel> GetResolvedTicketDetails(int queryId, string firebaseUid)
        {
            _logger.LogInformation($"Fetching resolved ticket details for QueryID: {queryId}");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);
            if (user == null)
            {
                _logger.LogError("User not found.");
                throw new Exception("User not found.");
            }

            // Fetch the query and resolution details
            var queryData = await _context.Queries
                .Where(q => q.QueryID == queryId)
                .Select(q => new ResolvedTicketAndQueryViewModel
                {
                    // Query Details
                    QueryID = q.QueryID,
                    TicketNumber = q.TicketNumber,
                    Description = q.Description,
                    SubmissionDate = q.SubmissionDate,
                    Status = q.Status,

                    // Student Details
                    StudentName = q.Student.User.Name + " " + q.Student.User.Surname,
                    StudentEmail = q.Student.User.Email,
                    DepartmentName = q.Department.DepartmentName,
                    CourseName = q.Course.CourseName,
                    Year = q.Student.Year,

                    // Resolved Ticket Details - Getting the first `QueryResolution` object related to the query
                    Solution = q.QueryResolutions.FirstOrDefault().Solution,
                    ApprovalStatus = q.QueryResolutions.FirstOrDefault().ApprovalStatus,
                    AdditionalNotes = q.QueryResolutions.FirstOrDefault().AdditionalNotes,
                    ResolutionDate = q.QueryResolutions.FirstOrDefault().ResolutionDate,


                    // Resolution Documents (Lazy-loaded)
                    Documents = q.QueryResolutions.FirstOrDefault().ResolutionDocuments
                        .Select(rd => new DocumentViewModel
                        {
                            DocumentPath = rd.QueryDocument.DocumentPath,
                            DocumentName = rd.QueryDocument.DocumentName,
                            UploadDate = rd.QueryDocument.UploadDate
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (queryData == null)
            {
                _logger.LogWarning("No query or resolution found for QueryID: {QueryID}", queryId);
                return null;
            }

            _logger.LogInformation("Successfully retrieved query and resolution details for QueryID: {QueryID}", queryId);

            return queryData;
        }




        public async Task<bool> SubmitFeedbackAsync(string firebaseUid, int queryId, int rating, string comments, bool isAnonymous)
        {
            _logger.LogInformation("Started SubmitFeedback for QueryID: {QueryID}, Rating: {Rating}, Anonymous: {IsAnonymous}", queryId, rating, isAnonymous);

            var user = await _context.Users
                .Include(u => u.StudentDetails)
                .FirstOrDefaultAsync(u => u.FirebaseUID == firebaseUid);

            if (user == null || !user.StudentDetails.Any())
            {
                _logger.LogError("No user or student details found for FirebaseUID: {FirebaseUID}", firebaseUid);
                return false;
            }

            var studentId = user.StudentDetails.First().StudentID;
            _logger.LogInformation("Fetched student details for StudentID: {StudentID}", studentId);

            var query = await _context.Queries.FirstOrDefaultAsync(q => q.QueryID == queryId && q.Status == QueryStatus.Resolved);

            if (query == null)
            {
                _logger.LogWarning("No resolved query found for QueryID: {QueryID}", queryId);
                return false;
            }

            _logger.LogInformation("Resolved query found for QueryID: {QueryID}", queryId);

            var feedback = new Feedback
            {
                QueryID = isAnonymous ? null : queryId,
                StudentID = isAnonymous ? null : studentId,
                Rating = rating,
                Comments = comments,
                SubmissionDate = DateTime.UtcNow,
                IsAnonymous = isAnonymous
            };

            _logger.LogInformation("Created feedback for QueryID: {QueryID}, IsAnonymous: {IsAnonymous}", isAnonymous ? "Anonymous" : queryId.ToString(), isAnonymous);

            _context.Feedback.Add(feedback);
            query.Status = QueryStatus.Closed;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Feedback submitted successfully for QueryID: {QueryID}, StudentID: {StudentID}, Anonymous: {IsAnonymous}", queryId, studentId, isAnonymous);

            return true;
        }
    }

}




