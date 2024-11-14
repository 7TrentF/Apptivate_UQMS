using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.DashboardStatsViewModel;
using static Apptivate_UQMS_WebApp.Models.Account.StudentDetail;
using static Apptivate_UQMS_WebApp.Models.Account;
namespace Apptivate_UQMS_WebApp.Models
{
    public class StudentDashboardViewModel
    {
        public IEnumerable<Query> RecentQueries { get; set; } // List of queries
        public DashboardStatsViewModel DashboardStats { get; set; } // Stats like Pending, Resolved, In Progress

        public StudentDetail StudentDetails { get; set; }

        public User Users { get; set; }

    }

}
