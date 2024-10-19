using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.DTOs.AccountDto;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.QueryModel;

namespace Apptivate_UQMS_WebApp.DTOs
{
    public class QueryModelDto
    {
        public class QueryDto
        {
            public int QueryID { get; set; }
            public int StudentID { get; set; }
            public int? DepartmentID { get; set; }
            public int? CourseID { get; set; }
            public int? ModuleID { get; set; }
            public int? Year { get; set; }
            public int QueryTypeID { get; set; }
            public int? CategoryID { get; set; }
            public string? Description { get; set; }
            public string? Status { get; set; }
            public DateTime? SubmissionDate { get; set; }
            public DateTime? ResolvedDate { get; set; }

            public FeedbackDto? Feedback { get; set; }
            public List<QueryDocumentDto> QueryDocuments { get; set; } = new List<QueryDocumentDto>();
            public QueryCategoryDto? Category { get; set; }
            public QueryTypeDto? QueryType { get; set; }
        }

        public enum QueryStatus
        {
            Pending,
            UnderReview,
            InProgress,
            OnHold,
            Resolved,
            Closed,
            Cancelled
        }

        public class QueryTypeDto
        {
            [Key]
            public int QueryTypeID { get; set; }
            public string TypeName { get; set; }

            public ICollection<QueryCategoryDto> QueryCategories { get; set; }
        }

        public class QueryCategoryDto
        {
            [Key]
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }

         //   public int QueryTypeID { get; set; } // Foreign Key
        //    public QueryType QueryType { get; set; }

          //  public ICollection<Query> Queries { get; set; }
        }

        public class AcademicQueryViewModel
        {
            public QueryType QueryType { get; set; }
            public StudentDetail StudentDetail { get; set; }
            public IEnumerable<QueryCategory> QueryCategories { get; set; }
        }

        public class SubmitQueryViewModelDto
        {
            public int QueryTypeID { get; set; }  // Add this property to pass the QueryTypeID
            public int? CategoryID { get; set; }
            public string Subject { get; set; }
            public int DepartmentID { get; set; }
            public int CourseID { get; set; }
            public int ModuleID { get; set; }
            public int Year { get; set; }
            public string QueryDetails { get; set; }
            public List<DepartmentDto> Departments { get; set; }
            public List<CourseDto> Courses { get; set; }
            public List<ModuleDto> Modules { get; set; }
            public List<string> QuerySubjects { get; set; }
        }

        public class QueryDocumentDto
        {
            [Key]
            public int DocumentID { get; set; }
            public int QueryID { get; set; } // Foreign Key
            public string DocumentName { get; set; }
            public string DocumentPath { get; set; }
            public DateTime UploadDate { get; set; }

            // Navigation property
            public QueryDto Query { get; set; }
        }

        public class QueryAssignmentDto
        {
            [Key]
            public int? AssignmentID { get; set; }
            public int? QueryID { get; set; } // Foreign Key
            public int? StaffID { get; set; } // Foreign Key
            public DateTime? AssignedDate { get; set; }
            public DateTime? ResolutionDate { get; set; }

            // Navigation properties
            public QueryDto? Query { get; set; }
            public StaffDetailDto? Staff { get; set; }
        }

        public class FeedbackDto
        {
            [Key]
            public int FeedbackID { get; set; }
            public int QueryID { get; set; } // Foreign Key
            public int StudentID { get; set; } // Foreign Key
            public int Rating { get; set; }
            public string Comments { get; set; }
            public DateTime SubmissionDate { get; set; }

            // Navigation properties
            public QueryDto Query { get; set; }
            public StudentDetailDto Student { get; set; }
        }

    }
}
