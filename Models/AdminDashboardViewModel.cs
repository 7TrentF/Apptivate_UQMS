// Models/AdminDashboardViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Apptivate_UQMS_WebApp.Models.Account;
using static Apptivate_UQMS_WebApp.Services.AnalyticsService;

namespace Apptivate_UQMS_WebApp.Models
{
    public class AdminDashboardViewModel
    {
        // Stats
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalQueries { get; set; }
        public int ResolvedQueries { get; set; }
        public int PendingQueries { get; set; }

        public int OngoingQueries { get; set; }
        public int ClosedQueries { get; set; }

        // Counts for different user roles
        public int StaffCount { get; set; }
        public int StudentCount { get; set; }
        public int AdminCount { get; set; }

        public User Users { get; set; }

        // User Management Overview
        public IEnumerable<UserManagementViewModel> UserManagementOverview { get; set; }

        // Weekly Activity Data (for charts)
        public IEnumerable<int> OpenQueries { get; set; }
        public IEnumerable<int> ReQueries { get; set; }

        // Card Expense Statistics Data
        public IEnumerable<int> CardExpenseData { get; set; }

        // Queries Received Data
        public IEnumerable<int> QueriesReceivedData { get; set; }

        // System Activity
        public IEnumerable<SystemActivityViewModel> SystemActivities { get; set; } = new HashSet<SystemActivityViewModel>();

        // Users List (for table)
        public IEnumerable<User> UsersList { get; set; }

        public TimeSpan AverageResponseTime { get; set; }
        public List<int> ResolvedQueryTrends { get; set; } // Monthly or Weekly Data

        public List<ResolutionRateDataPoint> ResolutionRates { get; set; }


        public List<ResponseTimeDataPoint> ResponseTimeTrends { get; set; }

    }

    public class ApplicationUsers
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class UserManagementViewModel
    {
        public string Role { get; set; }
        public int Count { get; set; }
        public int Percentage { get; set; }
    }

    public class SystemActivityViewModel
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public string Timestamp { get; set; }
    }

    public class QueryMetric
    {
        public int MetricID { get; set; }
        public DateTime Date { get; set; }
        public int TotalQueries { get; set; }
        public int ResolvedQueries { get; set; }
        public int PendingQueries { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan AverageResolutionTime { get; set; }
    }

 


}
