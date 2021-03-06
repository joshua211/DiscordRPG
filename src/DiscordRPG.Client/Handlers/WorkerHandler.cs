using DiscordRPG.Application.Worker;
using DiscordRPG.Common.Extensions;
using Hangfire;
using Serilog;

namespace DiscordRPG.Client.Handlers;

public class WorkerHandler : IHandler
{
    private readonly ILogger logger;

    public WorkerHandler(ILogger logger)
    {
        this.logger = logger.WithContext(GetType());
    }

    public Task InstallAsync()
    {
        logger.Here().Information("Installing WorkerHandler");
        RecurringJob.AddOrUpdate<CleaningWorker>("DungeonCleaner", x => x.RemoveExhaustedAndUnusedDungeons(),
            "1 * * * *");
        RecurringJob.AddOrUpdate<ShopWorker>("ShopUpdate", x => x.UpdateShopsAsync(), "0 */3 * * *");
        /*RecurringJob.AddOrUpdate<DiagnosticsWorker>("Diagnostics", subscriber => subscriber.FlushAsync(),
            "* * * * *");*/

        return Task.CompletedTask;
    }
}