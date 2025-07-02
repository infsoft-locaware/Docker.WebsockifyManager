using Infsoft.Docker.WebsockifyManager.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Infsoft.Docker.WebsockifyManager.Business
{
    /// <summary>
    /// Implements <see cref="ICache"/>
    /// </summary>
    internal class Cache : ICache
    {
        private Dictionary<Guid, WebsockifyConfig> LoadedConfigs { get; } = [];
        private Dictionary<Guid, Process> WebsockifyRefs { get; } = [];
        private Dictionary<Guid, int> Ports { get; } = [];

        /// <inheritdoc/>
        public bool TryGetConfig(Guid id, [NotNullWhen(true)] out WebsockifyConfig? config) => LoadedConfigs.TryGetValue(id, out config);
        /// <inheritdoc/>
        public bool TryGetProcess(Guid id, [NotNullWhen(true)] out Process? process) => WebsockifyRefs.TryGetValue(id, out process);

        /// <inheritdoc/>
        public void Set(WebsockifyConfig config, Process process)
        {
            LoadedConfigs[config.Id] = config;
            WebsockifyRefs[config.Id] = process;
        }

        /// <inheritdoc/>
        public void Remove(Guid id)
        {
            LoadedConfigs.Remove(id);
            WebsockifyRefs.Remove(id);
            Ports.Remove(id);
        }

        /// <inheritdoc/>
        public List<Guid> AllKeys => [.. LoadedConfigs.Keys];

        /// <inheritdoc/>
        public bool GetPort(Guid id, [NotNullWhen(true)] out int? port)
        {
            if (Ports.TryGetValue(id, out var p))
            {
                port = p;
                return true;
            }
            port = GetNextFreePort();
            if (port is null)
                return false;

            Ports[id] = port.Value;
            return true;
        }
    
        private static readonly IPEndPoint DefaultLoopbackEndpoint = new(IPAddress.Loopback, port: 0);
        private static int? GetNextFreePort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(DefaultLoopbackEndpoint);
            return ((IPEndPoint?)socket.LocalEndPoint)?.Port;
        }

    }
}
