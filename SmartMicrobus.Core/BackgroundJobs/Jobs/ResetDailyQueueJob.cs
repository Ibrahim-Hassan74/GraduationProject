using Hangfire.Console;
using Hangfire.Server;
using SmartMicrobus.Core.BackgroundJobs.Interfaces;
using SmartMicrobus.Core.ServiceContracts.Drivers;

namespace SmartMicrobus.Core.BackgroundJobs.Jobs
{
    public class ResetDailyQueueJob : IBackgroundJob
    {
        private readonly IDriverService _driverService;

        public ResetDailyQueueJob(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task ExecuteAsync(PerformContext context)
        {
            try
            {
                context.WriteLine("Starting ResetDailyQueueJob at: {0}", DateTimeOffset.Now);
                
                await _driverService.ResetDailyQueueAsync(context);
                
                context.WriteLine("Successfully completed ResetDailyQueueJob at: {0}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                context.WriteLine("An error occurred while executing ResetDailyQueueJob at: {0}", DateTimeOffset.Now);
                context.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
