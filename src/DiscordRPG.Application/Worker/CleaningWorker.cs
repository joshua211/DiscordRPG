using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;

namespace DiscordRPG.Application.Worker;

public class CleaningWorker
{
    private readonly IActivityService activityService;
    private readonly IChannelManager channelManager;
    private readonly IDungeonService dungeonService;
    private readonly ILogger logger;

    public CleaningWorker(IDungeonService dungeonService, ILogger logger, IChannelManager channelManager,
        IActivityService activityService)
    {
        this.dungeonService = dungeonService;
        this.channelManager = channelManager;
        this.activityService = activityService;
        this.logger = logger.WithContext(GetType());
    }

    public async Task RemoveExhaustedAndUnusedDungeons()
    {
        logger.Here().Information("Removing all exhausted Dungeons");
        var allDungeonResult = await dungeonService.GetAllDungeonsAsync();
        if (!allDungeonResult.WasSuccessful)
        {
            logger.Here().Error("Failed to get all dungeons, stopped cleaning");
            return;
        }

        var allActivities = await activityService.GetAllActivitiesAsync();
        if (!allActivities.WasSuccessful)
        {
            logger.Here().Error("Failed to get all activities, stopped cleaning");
            return;
        }

        var deletedCount = 0;
        foreach (var dungeon in allDungeonResult.Value)
        {
            if ((dungeon.ExplorationsLeft > 0 && !((DateTime.UtcNow - dungeon.LastModified).TotalHours >= 24)) ||
                allActivities.Value.Any(a => a.Data.ThreadId == dungeon.DungeonChannelId))
                continue;

            logger.Here()
                .Verbose(
                    "Removing dungeon {Id} because no more explorations are left or it was not used in 24 hours",
                    dungeon.ID);
            var deleteResult = await dungeonService.DeleteDungeonAsync(dungeon);
            if (!deleteResult.WasSuccessful)
                logger.Here().Error("Failed to delete dungeon with ID {Id}", dungeon.ID);

            logger.Here().Verbose("Deleting dungeon thread with ID {Id}", dungeon.DungeonChannelId);
            await channelManager.DeleteDungeonThreadAsync(dungeon.DungeonChannelId);
            deletedCount++;
        }

        logger.Here().Information("Finished cleaning and deleted {Count} dungeons", deletedCount);
    }
}