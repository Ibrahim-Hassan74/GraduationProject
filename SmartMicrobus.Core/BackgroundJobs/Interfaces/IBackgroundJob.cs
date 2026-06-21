using Hangfire.Server;

namespace SmartMicrobus.Core.BackgroundJobs.Interfaces
{
    public interface IBackgroundJob
    {
        Task ExecuteAsync(PerformContext context);
    }
}
