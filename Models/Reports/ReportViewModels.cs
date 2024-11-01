


using Apptivate_UQMS_WebApp.Migrations;

namespace Apptivate_UQMS_WebApp.Models.Reports
{
    public class ReportViewModels
    {



        public class QueryResolutionTimeReport
        {
            public string Category { get; set; }
            public string Department { get; set; }
            public double AverageResponseTime { get; set; }
            public double MedianResponseTime { get; set; }
            public DateTime Period { get; set; }
            public int QueryCount { get; set; }
        }

        public class HighVolumeQueryReport
        {
            public string Category { get; set; }
            public string Department { get; set; }
            public int QueryCount { get; set; }
            public double Percentage { get; set; }
            public List<MonthlyTrend> MonthlyTrends { get; set; }
        }

        public class MonthlyTrend
        {
            public DateTime Month { get; set; }
            public int Count { get; set; }
        }

        public class UnresolvedQueriesReport
        {
            public string Category { get; set; }
            public string Department { get; set; }
            public int TotalUnresolved { get; set; }
            public int TotalOverdue { get; set; }
            public double OverduePercentage { get; set; }
            public List<OverdueQuery> OverdueQueries { get; set; }
        }

        public class OverdueQuery
        {
            public int QueryId { get; set; }
         
            public string? TicketNumber { get; set; }
            public string Name { get; set; }
            public DateTime SubmissionDate { get; set; }
            public int PendingDays { get; set; }
        }

        public class StaffPerformanceReport
        {
            public string StaffName { get; set; }
            public string Department { get; set; }
            public int QueriesHandled { get; set; }
            public int QueriesResolved { get; set; }
            public double AverageResponseTime { get; set; }
            public double ResolutionRate { get; set; }
        }

    }
}
