namespace Infsoft.Docker.WebsockifyManager.Business
{
    /// <summary>
    /// Service to update websockify proxies
    /// </summary>
    internal interface IConfigUpdater
    {
        /// <summary>
        /// Update all available instances
        /// </summary>
        /// <param name="url">Url to query config from</param>
        /// <param name="apiKey">ApiKey to query with</param>
        Task Update(string url, string apiKey);
    }
}