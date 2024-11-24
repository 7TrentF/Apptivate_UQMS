using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.Reports.ReportViewModels;
using static Apptivate_UQMS_WebApp.Models.Account;
using Apptivate_UQMS_WebApp.Models;
using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.Models.StudentDashboardViewModel;

namespace Apptivate_UQMS_WebApp.Services
{
 


        public class ReportingService
        {
            private readonly ApplicationDbContext _context;
            private const int StandardResponseTime = 24; // hours

            public ReportingService(ApplicationDbContext context)
            {
                _context = context;
            }

        public async Task<List<QueryResolutionTimeReport>> GetQueryResolutionTimeReportAsync(
      DateTime startDate,
      DateTime endDate,
      string department)
        {
            var query = _context.Queries
                .Include(q => q.Category)
                .Include(q => q.Department)
                .Where(q => q.SubmissionDate >= startDate && q.SubmissionDate <= endDate);

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(q => q.Department.DepartmentName == department);
            }

            // First, get the basic aggregates
            var groupedQueries = await query
                .GroupBy(q => new { q.Category.CategoryName, q.Department.DepartmentName })
                .Select(g => new QueryResolutionTimeReport
                {
                    Category = g.Key.CategoryName,
                    Department = g.Key.DepartmentName,
                    QueryCount = g.Count(),
                    AverageResponseTime = g.Average(q =>
                        q.ResolvedDate.HasValue ? (double)
                        EF.Functions.DateDiffHour(q.SubmissionDate, q.ResolvedDate.Value) : 0.0),
                    ResponseTimes = g.Where(q => q.ResolvedDate.HasValue)
                        .Select(q => (double)EF.Functions.DateDiffHour(q.SubmissionDate, q.ResolvedDate.Value))
                        .OrderBy(t => t)
                        .ToList()
                })
                .ToListAsync();

            // Calculate median in memory
            foreach (var group in groupedQueries)
            {
                if (group.ResponseTimes.Any())
                {
                    var count = group.ResponseTimes.Count;
                    if (count % 2 == 0)
                    {
                        // If even number of items, average the two middle values
                        group.MedianResponseTime = (group.ResponseTimes[count / 2 - 1] + group.ResponseTimes[count / 2]) / 2;
                    }
                    else
                    {
                        // If odd number of items, take the middle value
                        group.MedianResponseTime = group.ResponseTimes[count / 2];
                    }
                }
                else
                {
                    group.MedianResponseTime = 0;
                }

                // Clean up the temporary list
                group.ResponseTimes = null;
            }

            return groupedQueries;
        }



        public async Task<List<HighVolumeQueryReport>> GetHighVolumeQueryReportAsync(
            DateTime startDate,
            DateTime endDate)
        {
            // Get total queries count
            var totalQueries = await _context.Queries
                .CountAsync(q => q.SubmissionDate >= startDate && q.SubmissionDate <= endDate);

            // First, get all relevant queries with their categories and departments
            var queries = await _context.Queries
                .Include(q => q.Category)
                .Include(q => q.Department)
                .Where(q => q.SubmissionDate >= startDate && q.SubmissionDate <= endDate)
                .Select(q => new
                {
                    CategoryName = q.Category.CategoryName,
                    DepartmentName = q.Department.DepartmentName,
                    SubmissionDate = q.SubmissionDate
                })
                .ToListAsync();

            // Group and calculate statistics in memory
            var report = queries
                .GroupBy(q => new { q.CategoryName, q.DepartmentName })
                .Select(g => new HighVolumeQueryReport
                {
                    Category = g.Key.CategoryName,
                    Department = g.Key.DepartmentName,
                    QueryCount = g.Count(),
                    Percentage = (double)g.Count() / totalQueries * 100,
                    MonthlyTrends = g
                        .GroupBy(q => new DateTime(q.SubmissionDate.Value.Year, q.SubmissionDate.Value.Month, 1))
                        .Select(m => new MonthlyTrend
                        {
                            Month = m.Key,
                            Count = m.Count()
                        })
                        .OrderBy(m => m.Month)
                        .ToList()
                })
                .OrderByDescending(r => r.QueryCount)
                .ToList();

            return report;
        }
        public async Task<UnresolvedQueriesReport> GetUnresolvedQueriesReportAsync()
            {
                var currentDate = DateTime.UtcNow;
                var overdueDate = currentDate.AddHours(-StandardResponseTime);

                var report = await _context.Queries
                    .Where(q => q.Status != QueryStatus.Resolved)
                    .GroupBy(q => new { q.Category.CategoryName, q.Department.DepartmentName })
                    .Select(g => new UnresolvedQueriesReport
                    {
                        Category = g.Key.CategoryName,
                        Department = g.Key.DepartmentName,
                        TotalUnresolved = g.Count(),
                        TotalOverdue = g.Count(q => q.SubmissionDate <= overdueDate),
                        OverduePercentage = (double)g.Count(q => q.SubmissionDate <= overdueDate) / g.Count() * 100,
                        OverdueQueries = g.Where(q => q.SubmissionDate <= overdueDate)
                            .Select(q => new OverdueQuery
                            {
                                TicketNumber = q.TicketNumber,
                                Name = q.Student.User.Name,
                                SubmissionDate = q.SubmissionDate.Value,
                                PendingDays = EF.Functions.DateDiffDay(q.SubmissionDate.Value, currentDate)
                            })
                            .OrderByDescending(q => q.PendingDays)
                            .ToList()
                    })
                    .ToListAsync();

                return report.FirstOrDefault();
            }

        public async Task<List<StaffPerformanceReport>> GetStaffPerformanceReportAsync(
 DateTime startDate,
 DateTime endDate)
        {
            var report = await _context.StaffDetails
                .Select(s => new StaffPerformanceReport
                {
                    StaffName = s.User.Name + " " + s.User.Surname,
                    //Department = s.Departments.DepartmentName,
                    QueriesHandled = s.QueryAssignments
                        .Count(qa => qa.Query != null &&
                                     qa.Query.SubmissionDate >= startDate &&
                                     qa.Query.SubmissionDate <= endDate),
                    QueriesResolved = s.QueryAssignments
                        .Count(qa => qa.Query != null &&
                                     qa.Query.Status == QueryStatus.Resolved &&
                                     qa.Query.ResolvedDate >= startDate &&
                                     qa.Query.ResolvedDate <= endDate),
                    AverageResponseTime = s.QueryAssignments
                        .Where(qa => qa.Query != null &&
                                     qa.Query.Status == QueryStatus.Resolved &&
                                     qa.Query.ResolvedDate >= startDate &&
                                     qa.Query.ResolvedDate <= endDate)
                        .Average(qa => EF.Functions.DateDiffHour(qa.Query.SubmissionDate, qa.Query.ResolvedDate.Value)) ?? 0.0,
                    ResolutionRate = s.QueryAssignments
                        .Count(qa => qa.Query != null &&
                                     qa.Query.SubmissionDate >= startDate &&
                                     qa.Query.SubmissionDate <= endDate) == 0
                        ? 0
                        : (double)s.QueryAssignments
                            .Count(qa => qa.Query != null &&
                                         qa.Query.Status == QueryStatus.Resolved &&
                                         qa.Query.ResolvedDate >= startDate &&
                                         qa.Query.ResolvedDate <= endDate) /
                          s.QueryAssignments
                            .Count(qa => qa.Query != null &&
                                         qa.Query.SubmissionDate >= startDate &&
                                         qa.Query.SubmissionDate <= endDate) * 100
                })
                .OrderByDescending(r => r.ResolutionRate)
                .ToListAsync();

            return report;
        }

    }
}

