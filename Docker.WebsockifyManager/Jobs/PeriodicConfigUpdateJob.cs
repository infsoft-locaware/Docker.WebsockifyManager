using Infsoft.Docker.WebsockifyManager.Business;
using Infsoft.Docker.WebsockifyManager.Models;
using Microsoft.Extensions.Options;

namespace Infsoft.Docker.WebsockifyManager.Jobs
{
    /// <summary>
    /// Update configuration in the configured intervall
    /// </summary>
    /// <param name="ServiceProvider">Provider to resolve services</param>
    /// <param name="Options">App configuration</param>
    public class PeriodicConfigUpdateJob(IServiceProvider ServiceProvider, IOptions<AppConfiguration> Options) : IHostedService, IDisposable
    {
        private Timer? _timer;

        private async void DoWork(object? _)
        {
            using var scope = ServiceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PeriodicConfigUpdateJob>>();
            var updater = scope.ServiceProvider.GetRequiredService<IConfigUpdater>();
            try
            {
                logger.LogInformation("Starting config sync");
                await updater.Update(Options.Value.Url, Options.Value.ApiKey);
                logger.LogInformation("Finished config sync");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed executing job");
            }
        }

        /// <inheritdoc/>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(Options.Value.IntervallInSeconds));

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose helper adhering to CA1816
        /// </summary>
        /// <param name="disposing">flag to indicate disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
        }
    }
}
