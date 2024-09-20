using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.DashboardStatsViewModel;

namespace Apptivate_UQMS_WebApp.Models
{
    public class StudentDashboardViewModel
    {
        public IEnumerable<Query> RecentQueries { get; set; } // List of queries
        public DashboardStatsViewModel DashboardStats { get; set; } // Stats like Pending, Resolved, In Progress
    }

}
