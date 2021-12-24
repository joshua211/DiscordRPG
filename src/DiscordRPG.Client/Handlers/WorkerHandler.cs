using DiscordRPG.Application.Worker;
using Hangfire;

namespace DiscordRPG.Client.Handlers;

public class WorkerHandler : IHandler
{
    public Task InstallAsync()
    {
        RecurringJob.AddOrUpdate<CleaningWorker>("DungeonCleaner", x => x.RemoveExhaustedAndUnusedDungeons(),
            "0 * * * *");
        RecurringJob.AddOrUpdate<DiagnosticsWorker>("Diagnostics", subscriber => subscriber.FlushAsync(),
            "* * * * *");
        RecurringJob.AddOrUpdate<ShopWorker>("ShopUpdate", x => x.UpdateShopsAsync(), "0 */3 * * *");

        return Task.CompletedTask;
    }
}