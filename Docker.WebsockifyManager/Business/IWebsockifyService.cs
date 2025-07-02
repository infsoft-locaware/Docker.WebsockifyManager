using Infsoft.Docker.WebsockifyManager.Models;
using System.Diagnostics;

namespace Infsoft.Docker.WebsockifyManager.Business
{
    /// <summary>
    /// Service to create and stop websockify instances
    /// </summary>
    internal interface IWebsockifyService
    {
        /// <summary>
        /// Start a websockify service
        /// </summary>
        /// <param name="config">Configuration to start with</param>
        /// <param name="port">Port to listen on</param>
        /// <returns>Websockify process reference</returns>
        Process? StartWebsockify(WebsockifyConfig config, int port);

        /// <summary>
        /// Stop the given process
        /// </summary>
        /// <param name="id">Configuration identifier</param>
        /// <param name="process">Process ref</param>
        void StopWebsockify(Guid id, Process process);
    }
}