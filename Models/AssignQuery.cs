namespace Apptivate_UQMS_WebApp.Models
{
    public class AssignQuery
    {

        public class TeamMemberModel
        {
            public int StaffID { get; set; }
            public string StaffName { get; set; }
            public string Position { get; set; }
            public string Department { get; set; }
            public List<string> QueryTypes { get; set; } = new List<string>();
        }

        public class AssignedQueryViewModel
        {
            public int QueryID { get; set; }
            public string Description { get; set; }
            public DateTime SubmissionDate { get; set; }
            public string Status { get; set; } // Current status of the query (e.g., Pending, In Progress, etc.)
            public List<string> QueryDocuments { get; set; } = new List<string>(); // List of document names/URLs
        }





    }
}
