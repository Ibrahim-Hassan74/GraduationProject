using Hangfire;
using SmartMicrobus.Core.BackgroundJobs.Interfaces;
using SmartMicrobus.Core.BackgroundJobs.Jobs;

namespace SmartMicrobus.Core.BackgroundJobs.Schedulers
{
    public class HangfireJobScheduler : IJobScheduler
    {
        public void RegisterJobs()
        {
            RecurringJob.AddOrUpdate<ResetDailyQueueJob>(
                "ResetDailyQueueAsync",
                job => job.ExecuteAsync(null),
                "0 0 * * *",
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo")
                });
        }
    }
}
