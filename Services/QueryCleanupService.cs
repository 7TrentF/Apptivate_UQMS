using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Apptivate_UQMS_WebApp.Services.QueryServices;

namespace Apptivate_UQMS_WebApp.Services
{
   

    public class QueryCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<QueryCleanupService> _logger;

        public QueryCleanupService(IServiceProvider serviceProvider, ILogger<QueryCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("QueryCleanupService is running at: {time}", DateTimeOffset.Now);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var queryService = scope.ServiceProvider.GetRequiredService<IQueryService>();

                    try
                    {
                        await queryService.CleanUpClosedQueriesAsync();
                        _logger.LogInformation("Query cleanup completed at: {time}", DateTimeOffset.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while cleaning up queries.");
                    }
                }

                // Wait for 24 hours before running the cleanup again
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }

}
