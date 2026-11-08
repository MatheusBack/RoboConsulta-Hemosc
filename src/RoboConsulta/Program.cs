using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using RoboConsulta.Configuration;
using RoboConsulta.Data;
using RoboConsulta.Services;
using RoboConsulta.Workers;

// Uso: RoboConsulta [worker|cron]  (padrão: worker)
var runOnce = args.FirstOrDefault()?.Equals("cron", StringComparison.OrdinalIgnoreCase) == true;

const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message:lj}{NewLine}{Exception}";

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, logger) => logger
        .MinimumLevel.Information()
        .WriteTo.Console(outputTemplate: logTemplate)
        .WriteTo.File(
            "logs/blood-search-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: logTemplate))
    .ConfigureServices((context, services) =>
    {
        services.Configure<BloodSearchOptions>(
            context.Configuration.GetSection(BloodSearchOptions.SectionName));

        services.AddSingleton<BloodSearchRepository>();
        services.AddTransient<BloodSearchJob>();

        services.AddHttpClient<HemoscClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("pt-BR,pt;q=0.9,en;q=0.7");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                AutomaticDecompression = DecompressionMethods.All,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            });

        if (!runOnce)
        {
            services.AddHostedService<BloodSearchWorker>();
        }
    })
    .Build();

if (runOnce)
{
    await host.Services.GetRequiredService<BloodSearchJob>().RunAsync();
}
else
{
    await host.RunAsync();
}
