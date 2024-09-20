// Models/AdminDashboardViewModel.cs
using System.Collections.Generic;
using static Apptivate_UQMS_WebApp.Models.Account;

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
        public IEnumerable<SystemActivityViewModel> SystemActivities { get; set; }
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
}
