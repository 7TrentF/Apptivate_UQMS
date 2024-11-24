// Controllers/LogsController.cs
using Apptivate_UQMS_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Apptivate_UQMS_WebApp.Models; // Import your models namespace
using System.IO;
using System.Threading.Tasks;
using static Apptivate_UQMS_WebApp.Models.LogsModel;

namespace Apptivate_UQMS_WebApp.Controllers
{

    public class LogsController : Controller
    {
        private readonly ILogger<LogsController> _logger;
        private readonly string _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> ViewLog(string logFileName)
        {
            if (string.IsNullOrEmpty(logFileName))
            {
                return BadRequest("Log file name cannot be empty.");
            }

            string logFilePath = Path.Combine(_logDirectory, logFileName);

            // Validate the path is within the logs directory to prevent directory traversal
            if (!logFilePath.StartsWith(_logDirectory, StringComparison.OrdinalIgnoreCase) ||
                !System.IO.File.Exists(logFilePath))
            {
                return NotFound("Log file not found.");
            }

            try
            {
                // Use FileShare.ReadWrite to allow reading while the file is being written to
                using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fileStream))
                {
                    var logContent = await reader.ReadToEndAsync();
                    return View("LogDetail", logContent);
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Error reading log file: {FileName}", logFileName);
                return View("Error", "Unable to read log file at this time. Please try again later.");
            }
        }

        public IActionResult Index()
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
                _logger.LogInformation("Created Logs directory at: {Path}", _logDirectory);
                return View(Array.Empty<LogEntry>());
            }

            try
            {
                var logFiles = Directory.GetFiles(_logDirectory, "*.txt")
                    .Select(filePath => new LogEntry
                    {
                        FileName = Path.GetFileName(filePath),
                        FilePath = filePath,
                        LastModified = System.IO.File.GetLastWriteTime(filePath) // Add last modified time
                    })
                    .OrderByDescending(log => log.LastModified) // Sort by most recent first
                    .ToArray();

                _logger.LogInformation("Found {Count} log files", logFiles.Length);
                return View(logFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing log files");
                return View(Array.Empty<LogEntry>());
            }
        }
    }

    // Update your LogEntry model to include LastModified
   
}