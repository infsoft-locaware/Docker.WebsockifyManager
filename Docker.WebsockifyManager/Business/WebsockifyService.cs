using Infsoft.Docker.WebsockifyManager.Models;
using System.Diagnostics;

namespace Infsoft.Docker.WebsockifyManager.Business
{
    /// <summary>
    /// Implements <see cref="IWebsockifyService"/>
    /// </summary>
    /// <param name="Logger">Logging instance</param>
    internal class WebsockifyService(ILogger<WebsockifyService> Logger) : IWebsockifyService
    {
        /// <inheritdoc/>
        public Process? StartWebsockify(WebsockifyConfig config, int port)
        {
            var process = Start(port, $"{config.Host}:{config.Port ?? 5901}");
            if (process is not null)
                Logger.LogInformation("Started websockify for {Id}", config.Id);
            else
                Logger.LogError("Failed to start websockify for {Id}", config.Id);

            return process;
        }

        /// <inheritdoc/>
        public void StopWebsockify(Guid id, Process process)
        {
            process.Exited -= LogCrash;
            process.Kill(true);
            Logger.LogInformation("Stopped websockify for {Id}", id);
        }

        private Process? Start(int port, string target)
        {
            var processInfo = new ProcessStartInfo("websockify", $"{port} {target}")
            {
                UseShellExecute = true,
                CreateNoWindow = true
            };
            using var process = new Process() { StartInfo = processInfo };
            process.Start();
            process.Exited += LogCrash;
            if (process.HasExited)
                return null;
            return process;
        }

        private void LogCrash(object? sender, EventArgs e)
        {
            Logger.LogError("Websockify crashed");
        }
    }
}
