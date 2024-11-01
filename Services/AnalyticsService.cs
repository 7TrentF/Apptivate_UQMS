using Apptivate_UQMS_WebApp.Data;
using Apptivate_UQMS_WebApp.Data;
using Microsoft.EntityFrameworkCore;
using static Apptivate_UQMS_WebApp.Models.QueryModel;
using static Apptivate_UQMS_WebApp.DTOs.QueryModelDto;
using QueryStatus = Apptivate_UQMS_WebApp.Models.QueryModel.QueryStatus;
using Apptivate_UQMS_WebApp.ViewModels;
using static Apptivate_UQMS_WebApp.ViewModels.QueryViewModel;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Apptivate_UQMS_WebApp.Models;

namespace Apptivate_UQMS_WebApp.Services
{
    public class AnalyticsService
    {
         private readonly ApplicationDbContext _context;

            public AnalyticsService(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<AdminDashboardViewModel> GetDashboardMetricsAsync()
            {
                var totalQueries = await _context.Queries.CountAsync();
                var resolvedQueries = await _context.Queries.CountAsync(q => q.Status == QueryStatus.Resolved);
                var pendingQueries = totalQueries - resolvedQueries;

                var averageResponseTime = await _context.Queries
                    .Where(q => q.Status == QueryStatus.Resolved)
                    .AverageAsync(q => EF.Functions.DateDiffMinute(q.SubmissionDate, q.ResolvedDate.Value));

                var viewModel = new AdminDashboardViewModel
                {
                    TotalQueries = totalQueries,
                    ResolvedQueries = resolvedQueries,
                    PendingQueries = pendingQueries,
                    AverageResponseTime = TimeSpan.FromMinutes((double)averageResponseTime)
                };

                return viewModel;
            }



        public async Task<List<ResponseTimeDataPoint>> GetResponseTimeTrendsAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            var responseTimeTrends = await _context.Queries
                .Where(q => q.Status == QueryStatus.Resolved && q.ResolvedDate >= startDate)
                .GroupBy(q => q.ResolvedDate.Value.Date)
                .Select(g => new ResponseTimeDataPoint
                {
                    Date = g.Key,
                    AverageResponseTime = g.Average(q =>
                        EF.Functions.DateDiffMinute(q.SubmissionDate, q.ResolvedDate.Value)) ?? 0
                })
                .OrderBy(r => r.Date)
                .ToListAsync();

            // Fill in missing dates with zero values
            var allDates = Enumerable.Range(0, days)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .OrderBy(date => date);

            var result = allDates.GroupJoin(
                responseTimeTrends,
                date => date,
                trend => trend.Date,
                (date, trends) => new ResponseTimeDataPoint
                {
                    Date = date,
                    AverageResponseTime = trends.FirstOrDefault()?.AverageResponseTime ?? 0
                }).ToList();

            return result;
        }


        public async Task<List<ResolutionRateDataPoint>> GetQueryResolutionRatesAsync(int days)
        {
            var startDate = DateTime.UtcNow.AddDays(-days);

            // Get all queries within the time period
            var queries = await _context.Queries
                .Where(q => q.SubmissionDate >= startDate)
                .Select(q => new
                {
                    SubmissionDate = q.SubmissionDate.Value.Date,
                    IsResolved = q.Status == QueryStatus.Resolved,
                    ResolvedDate = q.ResolvedDate.HasValue ? q.ResolvedDate.Value.Date : (DateTime?)null
                })
                .ToListAsync();

            // Group by date and calculate rates
            var resolutionRates = Enumerable.Range(0, days)
                .Select(offset => DateTime.UtcNow.Date.AddDays(-offset))
                .Select(date => new ResolutionRateDataPoint
                {
                    Date = date,
                    TotalQueries = queries.Count(q => q.SubmissionDate <= date),
                    ResolvedQueries = queries.Count(q => q.IsResolved && q.ResolvedDate <= date),
                    ResolutionRate = 0 // We'll calculate this next
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Calculate resolution rates
            foreach (var point in resolutionRates)
            {
                point.ResolutionRate = point.TotalQueries > 0
                    ? (double)point.ResolvedQueries / point.TotalQueries * 100
                    : 0;
            }

            return resolutionRates;
        }






        // AnalyticsService.cs
        public class ResponseTimeDataPoint
        {
            public DateTime Date { get; set; }
            public double AverageResponseTime { get; set; }
        }



        // Add this to your AnalyticsService.cs
        public class ResolutionRateDataPoint
        {
            public DateTime Date { get; set; }
            public int TotalQueries { get; set; }
            public int ResolvedQueries { get; set; }
            public double ResolutionRate { get; set; }
        }


    }





}
