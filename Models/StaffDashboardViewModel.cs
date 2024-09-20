using static Apptivate_UQMS_WebApp.Models.DashboardStatsViewModel;
using static Apptivate_UQMS_WebApp.Models.Account.StudentDetail;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Models.QueryModel;

namespace Apptivate_UQMS_WebApp.Models
{
    public class StaffDashboardViewModel
    {
        public IEnumerable<QueryAssignment> AssignedQueries { get; set; } // List of assigned queries
        public DashboardStatsViewModel DashboardStats { get; set; } // Stats like Pending, Resolved, In Progress
        public StaffDetail StaffDetails { get; set; }
        public User Users { get; set; }
        public IEnumerable<TeamMemberViewModel> TeamOverview { get; set; } // New property for Team Overview
    }


    public class TeamMemberViewModel
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public List<string> QueryTypes { get; set; } = new List<string>();
    }




}
