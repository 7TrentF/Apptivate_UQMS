using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Models.Account;
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

            public int? CategoryID { get; set; } // Foreign Key to QueryCategory
            public QueryCategory Category { get; set; }

            public string? Status { get; set; }
            public DateTime? SubmissionDate { get; set; }
            public DateTime? ResolvedDate { get; set; }

            // Navigation properties
            public StudentDetail Student { get; set; }
            public Department Department { get; set; }
            public Course Course { get; set; }
            public Module Module { get; set; }
            public ICollection<QueryDocument> QueryDocuments { get; set; }
            public ICollection<QueryAssignment> QueryAssignments { get; set; }
            public Feedback? Feedback { get; set; }
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
            public int StudentID { get; set; }
            public string Category { get; set; }  // Academic or Administrative
            public string Subject { get; set; }  // Specific subject (e.g., "Grade remarks")
            public int DepartmentID { get; set; }
            public int CourseID { get; set; }
            public int ModuleID { get; set; }
            public int Year { get; set; }
            public string QueryDetails { get; set; }

            public List<Department> Departments { get; set; }
            public List<Course> Courses { get; set; }
            public List<Module> Modules { get; set; }
            public List<string> QuerySubjects { get; set; }  // Predefined query subjects
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
        }


        public class QueryAssignment
        {
            [Key]
            public int AssignmentID { get; set; }
            public int QueryID { get; set; } // Foreign Key
            public int StaffID { get; set; } // Foreign Key
            public DateTime AssignedDate { get; set; }
            public DateTime? ResolutionDate { get; set; }

            // Navigation properties
            public Query Query { get; set; }
            public StaffDetail Staff { get; set; }
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

      

    }
}
