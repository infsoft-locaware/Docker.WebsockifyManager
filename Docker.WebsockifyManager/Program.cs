using Infsoft.Docker.WebsockifyManager;
using Infsoft.Docker.WebsockifyManager.Jobs;
using System.Security.Cryptography.X509Certificates;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>().ConfigureKestrel(kestrel =>
{
    kestrel.AddServerHeader = false;
    kestrel.ListenAnyIP(443, options =>
    {
        var cert = X509Certificate2.CreateFromPemFile("/ssl/cert.pem", "/ssl/cert.key");
        if (cert is null)
        {
            Console.Error.WriteLine("Could not load certificate");
            Environment.Exit(2);
        }
        options.UseHttps(cert);
    });
}));
builder.ConfigureServices(services => services.AddHostedService<PeriodicConfigUpdateJob>());

var app = builder.Build();
app.Run();
