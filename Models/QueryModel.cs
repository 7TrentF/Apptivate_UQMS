using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Apptivate_UQMS_WebApp.Models
{
    public class QueryModel
    {
        public class Query
        {
            [Key]
            public int QueryID { get; set; }
            public int StudentID { get; set; } // Foreign Key
            public int? DepartmentID { get; set; } // Foreign Key
            public int? CourseID { get; set; } // Foreign Key
            public int? ModuleID { get; set; } // Foreign Key
            public int? Year { get; set; }
            public int? QueryTypeID { get; set; }
            public int? CategoryID { get; set; } // Foreign Key to QueryCategory
            public QueryCategory? Category { get; set; }

            [StringLength(150, ErrorMessage = "Description cannot be longer than 150 characters.")]
            public string? Description { get; set; }
            public QueryStatus Status { get; set; } // Default to Pending on creation

            public DateTime? SubmissionDate { get; set; }
            public DateTime? ResolvedDate { get; set; }

            // Navigation properties
            public StudentDetail? Student { get; set; }
            public Department? Department { get; set; }
            public Course? Course { get; set; }
            public Module? Module { get; set; }
            public ICollection<QueryDocument> QueryDocuments { get; set; } = new HashSet<QueryDocument>();
            public ICollection<QueryAssignment> QueryAssignments { get; set; } = new HashSet<QueryAssignment>();
            public ICollection<QueryResolutions> QueryResolutions { get; set; } = new HashSet<QueryResolutions>();

            public Feedback? Feedback { get; set; }
        }

        public enum QueryStatus
        {
            Pending,     // Query submitted but not yet assigned
            Ongoing,     // Query assigned to staff and in progress
            Resolved,    // Query has been resolved
            Closed       // Query is closed and requires no further action
        }


        public class QueryCategory
        {
            [Key]
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }

            public int QueryTypeID { get; set; } // Foreign Key
            public QueryType QueryType { get; set; }

            public ICollection<Query> Queries { get; set; }
        }

        public class QueryType
        {
            [Key]
            public int QueryTypeID { get; set; }
            public string TypeName { get; set; }

            public ICollection<QueryCategory> QueryCategories { get; set; }
        }



        public class SubmitQueryViewModel
        {
            public int QueryTypeID { get; set; }  // Add this property to pass the QueryTypeID
            public int? CategoryID { get; set; }
            public string Subject { get; set; }
            public int DepartmentID { get; set; }
            public int CourseID { get; set; }
            public int ModuleID { get; set; }
            public int Year { get; set; }
            public string QueryDetails { get; set; }

            public List<Department> Departments { get; set; }
            public List<Course> Courses { get; set; }
            public List<Module> Modules { get; set; }
            public List<string> QuerySubjects { get; set; }
        }



        public class QueryDocument
        {
            [Key]
            public int DocumentID { get; set; }
            public int QueryID { get; set; } // Foreign Key
            public string DocumentName { get; set; }
            public string DocumentPath { get; set; }
            public DateTime UploadDate { get; set; }

            // Navigation property
            public Query Query { get; set; }
            public ICollection<ResolutionDocuments> ResolutionDocuments { get; set; } = new HashSet<ResolutionDocuments>();

        }


        public class QueryAssignment
        {
            [Key]
            public int? AssignmentID { get; set; }
            public int? QueryID { get; set; } // Foreign Key
            public int? StaffID { get; set; } // Foreign Key
            public DateTime? AssignedDate { get; set; }
            public DateTime? ResolutionDate { get; set; }

            // Navigation properties
            public Query? Query { get; set; }
            public StaffDetail? Staff { get; set; }

            public ICollection<QueryResolutions> QueryResolutions { get; set; } = new HashSet<QueryResolutions>();

        }

        public class Feedback
        {
            [Key]
            public int FeedbackID { get; set; }
            public int QueryID { get; set; } // Foreign Key
            public int StudentID { get; set; } // Foreign Key
            public int Rating { get; set; }
            public string Comments { get; set; }
            public DateTime SubmissionDate { get; set; }

            // Navigation properties
            public Query Query { get; set; }
            public StudentDetail Student { get; set; }
        }


        public class QueryResolutions
        {
            [Key]
            public int ResolutionID { get; set; }  // Primary Key

            public int AssignmentID { get; set; }  // Foreign Key to QueryAssignments
            public int QueryID { get; set; }       // Foreign Key to Queries

            [Required]
            public string Solution { get; set; }   // The solution provided by staff

            public string? ApprovalStatus { get; set; }   // The solution provided by staff

            public string? AdditionalNotes { get; set; } // Nullable additional notes

            public DateTime?  ResolutionDate { get; set; }


            // Navigation properties
            public QueryAssignment? Assignment { get; set; }  // Link to QueryAssignment
            public Query? Query { get; set; }  // Link to Query

            public ICollection<ResolutionDocuments> ResolutionDocuments { get; set; } = new HashSet<ResolutionDocuments>();
        }

        public class ResolutionDocuments
        {
            public int ResolutionID { get; set; }  // Foreign Key to QueryResolution
            public int DocumentID { get; set; }    // Foreign Key to QueryDocuments

            // Navigation properties
            public QueryResolutions QueryResolution { get; set; }
            public QueryDocument QueryDocument { get; set; }
        }

        public class ResolvedTicketViewModel
        {
            public int ResolutionID { get; set; }
            public int AssignmentID { get; set; }
            public int QueryID { get; set; }
            public string Solution { get; set; }
            public string ApprovalStatus { get; set; }
            public string AdditionalNotes { get; set; }
            public List<DocumentViewModel> Documents { get; set; }
        }

        public class DocumentViewModel
        {
            public string DocumentPath { get; set; }
            public string DocumentName { get; set; }
            public DateTime UploadDate { get; set; }
        }


        public class ResolvedTicketAndQueryViewModel
        {
            // Query Details
            public int QueryID { get; set; }
            public string? Description { get; set; }
            public DateTime? SubmissionDate { get; set; }
            public QueryStatus Status { get; set; }
            // Student Details
            public string StudentName { get; set; }
            public string StudentEmail { get; set; }
            public string DepartmentName { get; set; }
            public string CourseName { get; set; }
            public int? Year { get; set; }

            // Resolved Ticket Details
            public string Solution { get; set; }
            public string ApprovalStatus { get; set; }
            public string AdditionalNotes { get; set; }

            public DateTime?  ResolutionDate { get; set; }
            public List<DocumentViewModel> Documents { get; set; }
        }

      


    }
}
