using static Apptivate_UQMS_WebApp.Models.QueryModel;
namespace Apptivate_UQMS_WebApp.Models
{
    public class DashboardStatsViewModel
    {
        public int PendingCount { get; set; }
        public int ResolvedCount { get; set; }
        public int InProgressCount { get; set; }
    }

}
