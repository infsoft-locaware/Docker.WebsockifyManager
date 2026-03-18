using Infsoft.Docker.WebsockifyManager;
using Infsoft.Docker.WebsockifyManager.Jobs;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>().ConfigureKestrel(kestrel =>
{
    kestrel.AddServerHeader = false;
    kestrel.ListenAnyIP(8080);
        
}));
builder.ConfigureServices(services => services.AddHostedService<PeriodicConfigUpdateJob>());

var app = builder.Build();
await app.RunAsync();
