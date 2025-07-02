using System.Text.Json.Serialization;

namespace Infsoft.Docker.WebsockifyManager.Models
{
    /// <summary>
    /// Api response for config queries, not all instances must be valid
    /// </summary>
    internal class WebsockifyConfigApi
    {
        /// <summary>
        /// Object identifier (unique)
        /// </summary>
        [JsonPropertyName("uid")]
        public Guid? Id { get; set; }

        /// <summary>
        /// Host to proxy to
        /// </summary>
        [JsonPropertyName("websockifyHost")]
        public string? Host { get; set; }

        /// <summary>
        /// Port to proxy to
        /// </summary>
        [JsonPropertyName("websockifyPort")]
        public int? Port { get; set; }
    }
}
