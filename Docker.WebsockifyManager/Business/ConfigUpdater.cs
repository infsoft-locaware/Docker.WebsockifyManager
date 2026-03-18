using Infsoft.Docker.WebsockifyManager.Models;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace Infsoft.Docker.WebsockifyManager.Business;

/// <summary>
/// Implements <see cref="IConfigUpdater"/>
/// </summary>
/// <param name="proxyConfigProvider">YARP InMemory Reverse Proxy configuration</param>
/// <param name="httpClient">Generic HttpClient to fetch configuration</param>
/// <param name="cache">Cache for loaded configuration and process refs</param>
/// <param name="websockifyService">Service to create and stop websockify instances</param>
/// <param name="logger">Logging instance</param>
internal class ConfigUpdater(InMemoryConfigProvider proxyConfigProvider, HttpClient httpClient, ICache cache, IWebsockifyService websockifyService, ILogger<ConfigUpdater> logger) : IConfigUpdater
{
    /// <inheritdoc/>
    public async Task Update(string url, string apiKey)
    {
        var configs = await FetchConfigs(url, apiKey);
        if (configs is null)
        {
            logger.LogError("Could not load configuration data, aborting");
            return;
        }

        if (configs.Count == 0)
        {
            logger.LogWarning("No configs found, skipping");
            return;
        }

        var routes = new List<RouteConfig>();
        var clusters = new List<ClusterConfig>();
        var toDelete = cache.AllKeys.Except(configs.Select(config => config.Id)).ToList();
        foreach (var instance in toDelete)
            DeleteWebsockifyInstance(instance);

        logger.LogInformation("Deleted {Count} configurations", toDelete.Count);
        var updated = 0;
        foreach (var config in configs)
        {
            if (!cache.GetPort(config.Id, out var port))
            {
                logger.LogError("No free port available");
                continue;
            }
            var exists = false;
            if (cache.TryGetConfig(config.Id, out var conf))
                if (conf.Host == config.Host && conf.Port == config.Port)
                    exists = true;
                else
                    DeleteWebsockifyInstance(config.Id);

            if (exists || CreateWebsockifyInstance(config, port.Value))
            {
                if (!exists) updated++;
                routes.Add(CreateRoute(config));
                clusters.Add(CreateCluster(config, port.Value));
            }
            else
                logger.LogWarning("Could not create websockify instance for config {Id}", config.Id);
        }
        logger.LogInformation("Updated {Count} of {Total} configurations", updated, routes.Count);
        proxyConfigProvider.Update(routes, clusters);
    }

    private static RouteConfig CreateRoute(WebsockifyConfig config)
    {
        var routeConfig = new RouteConfig()
        {
            RouteId = config.Id.ToString(),
            ClusterId = config.Id.ToString(),
            Match = new()
            {
                Path = $"/proxy/{config.Host}"
            }
        }.WithTransformPathRemovePrefix($"/proxy/{config.Host}");

        return routeConfig;
    }

    private static ClusterConfig CreateCluster(WebsockifyConfig config, int port) => new()
    {
        ClusterId = config.Id.ToString(),
        Destinations = new Dictionary<string, DestinationConfig>(StringComparer.OrdinalIgnoreCase)
        {
            { config.Id.ToString(), new() { Address = $"ws://localhost:{port}" } }
        }
    };

    private async Task<List<WebsockifyConfig>?> FetchConfigs(string url, string apiKey)
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new(url)
        };
        request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
        var response = await httpClient.SendAsync(request);
        var configs = await response.Content.ReadFromJsonAsync<List<WebsockifyConfigApi>>();
        return configs is null ? null : [.. configs.Where(c => c.Id is not null && c.Host is not null && (c.Port is null || c.Port > 0)).Select(c => new WebsockifyConfig() { Host = c.Host!, Id = c.Id!.Value, Port = c.Port })];
    }

    private void DeleteWebsockifyInstance(Guid id)
    {
        if (cache.TryGetProcess(id, out var process))
        {
            websockifyService.StopWebsockify(id, process);
            cache.Remove(id);
        }
        else
            logger.LogWarning("Process for config {Id} not found, cannot stop", id);
    }

    private bool CreateWebsockifyInstance(WebsockifyConfig config, int port)
    {
        var process = websockifyService.StartWebsockify(config, port);
        if (process is not null)
            cache.Set(config, process);
        return process is not null;
    }
}