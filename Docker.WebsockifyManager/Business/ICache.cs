using Infsoft.Docker.WebsockifyManager.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Infsoft.Docker.WebsockifyManager.Business
{
    /// <summary>
    /// Cache for configuration, used ports and process refs
    /// </summary>
    internal interface ICache
    {
        /// <summary>
        /// All configuration identifiers currently in the cache
        /// </summary>
        List<Guid> AllKeys { get; }

        /// <summary>
        /// Check, whether the given config with <paramref name="id"/> is present in the cache
        /// </summary>
        /// <param name="id">Configuration identifier</param>
        /// <param name="config">Configuration, if found</param>
        /// <returns><see langword="true"/>, when config is found</returns>
        bool TryGetConfig(Guid id, [NotNullWhen(true)] out WebsockifyConfig? config);

        /// <summary>
        /// Query a port for given config
        /// </summary>
        /// <param name="id">Configuration identifier</param>
        /// <param name="port">Port to use for given configuration</param>
        /// <returns><see langword="false"/>, should no port be available</returns>
        bool GetPort(Guid id, [NotNullWhen(true)] out int? port);

        /// <summary>
        /// Check, whether the given process with <paramref name="id"/> is present in the cache
        /// </summary>
        /// <param name="id">Configuration identifier</param>
        /// <param name="process">Process, if found</param>
        /// <returns><see langword="true"/>, when process is found</returns>
        bool TryGetProcess(Guid id, [NotNullWhen(true)] out Process? process);

        /// <summary>
        /// Remove process, configuration and used host from cache
        /// </summary>
        /// <param name="id"></param>
        void Remove(Guid id);

        /// <summary>
        /// Store the given <paramref name="config"/> and <paramref name="process"/> in the cache
        /// </summary>
        /// <param name="config">Configuration to cache</param>
        /// <param name="process">Process ref to cache</param>
        void Set(WebsockifyConfig config, Process process);
    }
}