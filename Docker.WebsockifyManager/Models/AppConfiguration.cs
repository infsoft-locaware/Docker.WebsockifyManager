namespace Infsoft.Docker.WebsockifyManager.Models
{
    /// <summary>
    /// App configuration, will be created based on environment variables
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// Url to query in order to fetch configuration
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// ApiKey required to call the api
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Interval in which the config is queried and refreshed at (in seconds)
        /// </summary>
        public int IntervallInSeconds { get; set; } = 0;

        /// <summary>
        /// Checks, whether all properties have valid values
        /// </summary>
        public bool IsInvalid => Url == string.Empty || ApiKey == string.Empty || IntervallInSeconds <= 0;
    }
}
