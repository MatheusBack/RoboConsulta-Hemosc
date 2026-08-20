using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using RoboConsulta.Data;
using RoboConsulta.Services;
using RoboConsulta.Workers;

var mode = args.FirstOrDefault()?.ToLower() ?? "worker";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var connectionString = "Data Source=blood_search.db";

        services.AddSingleton<DatabaseContext>(new DatabaseContext(connectionString));
        services.AddHttpClient<HemoscService>();
        services.AddScoped<CronRunner>();

        if (mode == "worker" || mode == "daemon")
        {
            services.AddHostedService<BloodSearchWorker>();
        }
    })
    .UseSerilog((context, loggerConfig) =>
    {
        loggerConfig
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/blood-search-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] {Message:lj}{NewLine}{Exception}",
                retainedFileCountLimit: 7);
    })
    .Build();

if (mode == "cron")
{
    var cronRunner = host.Services.GetRequiredService<CronRunner>();
    await cronRunner.ExecuteAsync();
}
else
{
    await host.RunAsync();
}
