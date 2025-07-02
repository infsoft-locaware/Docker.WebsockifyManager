namespace Infsoft.Docker.WebsockifyManager.Models
{
    /// <summary>
    /// Valid websockify configuration
    /// </summary>
    internal class WebsockifyConfig
    {
        /// <summary>
        /// Object identifier (unique)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Host to proxy to
        /// </summary>
        public required string Host { get; set; }

        /// <summary>
        /// Port to proxy to
        /// </summary>
        public int? Port { get; set; }
    }
}
