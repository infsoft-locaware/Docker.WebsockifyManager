using Infsoft.Docker.WebsockifyManager.Business;
using Infsoft.Docker.WebsockifyManager.Models;

namespace Infsoft.Docker.WebsockifyManager
{
    public class Startup
    {
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="services">service collection to fill</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppConfiguration>(config =>
            {
                var interval = Environment.GetEnvironmentVariable("CONFIG_RELOAD_INTERVALL") ?? "3600";
                if (int.TryParse(interval, out var configInterval))
                    config.IntervallInSeconds = configInterval;
                else
                    Console.Error.WriteLine("Invalid interval: {interval}, must be integer");
                var apiKey = Environment.GetEnvironmentVariable("CONFIG_API_KEY");
                if (apiKey is not null)
                    config.ApiKey = apiKey;
                else
                    Console.Error.WriteLine("No apiKey defined");

                var url = Environment.GetEnvironmentVariable("CONFIG_API_URL");
                if (url is not null)
                    config.Url = url;
                else
                    Console.Error.WriteLine("No service url defined");

                if (config.IsInvalid)
                    Environment.Exit(1);
            });

            services.AddHttpClient();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddReverseProxy().LoadFromMemory([], []);
            services.AddTransient<IConfigUpdater, ConfigUpdater>();
            services.AddTransient<IWebsockifyService, WebsockifyService>();
            services.AddSingleton<ICache, Cache>();
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">app to build</param>
        /// <param name="env">environment to build app for</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapReverseProxy());
        }
    }
}
