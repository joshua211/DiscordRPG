using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;

namespace DiscordRPG.Application.Worker;

public class CleaningWorker
{
    private readonly IActivityService activityService;
    private readonly IChannelManager channelManager;
    private readonly IDungeonService dungeonService;
    private readonly IGuildService guildService;
    private readonly ILogger logger;

    public CleaningWorker(IDungeonService dungeonService, ILogger logger, IChannelManager channelManager,
        IActivityService activityService, IGuildService guildService)
    {
        this.dungeonService = dungeonService;
        this.channelManager = channelManager;
        this.activityService = activityService;
        this.guildService = guildService;
        this.logger = logger.WithContext(GetType());
    }

    public async Task RemoveExhaustedAndUnusedDungeons()
    {
        var context = TransactionContext.New();
        logger.Context(context).Information("Removing all exhausted Dungeons");

        var guilds = await guildService.GetAllGuildsAsync(context);
        if (!guilds.WasSuccessful)
        {
            logger.Context(context).Warning("Failed to get all guilds, cant clean");
            return;
        }

        var allActivities = await activityService.GetAllActivitiesAsync(context);
        if (!allActivities.WasSuccessful)
        {
            logger.Context(context).Warning("Failed to get all activities, cant clean");
            return;
        }

        foreach (var guild in guilds.Value)
        {
            var guildDungeons = await dungeonService.GetAllDungeonsAsync(new GuildId(guild.Id), context);
            if (!guildDungeons.WasSuccessful)
            {
                logger.Context(context).Warning("Failed to get dungeons for Guild {Id}, cant clean", guild.Id);
                return;
            }

            var deletedCount = 0;
            foreach (var dungeon in guildDungeons.Value)
            {
                if (dungeon.Explorations.Value > 0 && !((DateTime.UtcNow - dungeon.LastInteraction).TotalHours >= 24) ||
                    allActivities.Value.Any(a => a.ActivityData.DungeonId == new DungeonId(dungeon.Id)))
                    continue;

                logger.Context(context)
                    .Verbose(
                        "Removing dungeon {Id} because no more explorations are left or it was not used in 24 hours",
                        dungeon.Id);
                var deleteResult =
                    await dungeonService.DeleteDungeonAsync(new GuildId(guild.Id), new DungeonId(dungeon.Id), context);
                if (!deleteResult.WasSuccessful)
                    logger.Context(context).Error("Failed to delete dungeon with ID {Id}", dungeon.Id);

                logger.Context(context).Verbose("Deleting dungeon thread with ID {Id}", dungeon.Id);
                await channelManager.DeleteDungeonThreadAsync(new ChannelId(dungeon.Id), context);
                deletedCount++;
            }

            logger.Context(context).Information("Finished cleaning and deleted {Count} dungeons", deletedCount);
        }
    }
}