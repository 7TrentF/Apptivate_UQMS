﻿using Microsoft.AspNetCore.Mvc.Rendering;
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
            public string Category { get; set; }
            public int DepartmentID { get; set; } // Foreign Key
            public int CourseID { get; set; } // Foreign Key
            public int ModuleID { get; set; } // Foreign Key
            public int Year { get; set; }
            public string Status { get; set; }
            public DateTime SubmissionDate { get; set; }
            public DateTime? ResolvedDate { get; set; }

            // Navigation properties
            public StudentDetail Student { get; set; }
            public Department Department { get; set; }
            public Course Course { get; set; }
            public Module Module { get; set; }
            public ICollection<QueryDocument> QueryDocuments { get; set; }
            public ICollection<QueryAssignment> QueryAssignments { get; set; }
            public Feedback Feedback { get; set; }
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
