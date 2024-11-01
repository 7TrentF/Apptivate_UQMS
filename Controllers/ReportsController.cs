using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apptivate_UQMS_WebApp.Controllers
{
    // Controllers/ReportsController.cs
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ReportingService _reportingService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            ReportingService reportingService,
            ILogger<ReportsController> logger)
        {
            _reportingService = reportingService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> QueryResolutionTime(DateTime? startDate, DateTime? endDate, string department = null)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var report = await _reportingService.GetQueryResolutionTimeReportAsync(startDate.Value, endDate.Value, department);
            return Json(report);
        }

        [HttpGet]
        public async Task<IActionResult> HighVolumeQueries(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var report = await _reportingService.GetHighVolumeQueryReportAsync(startDate.Value, endDate.Value);
            return Json(report);
        }

        [HttpGet]
        public async Task<IActionResult> UnresolvedQueries()
        {
            var report = await _reportingService.GetUnresolvedQueriesReportAsync();
            return Json(report);
        }

        [HttpGet]
        public async Task<IActionResult> StaffPerformance(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var report = await _reportingService.GetStaffPerformanceReportAsync(startDate.Value, endDate.Value);
            return Json(report);
        }
    }
}
