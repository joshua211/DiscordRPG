using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Core.Commands.Activities;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Worker;

public class ActivityWorker
{
    private readonly IChannelManager channelManager;
    private readonly IGuildService guildService;
    private readonly ILogger logger;
    private readonly IMediator mediator;

    public ActivityWorker(IMediator mediator, IChannelManager channelManager, IGuildService guildService,
        ILogger logger)
    {
        this.mediator = mediator;
        this.channelManager = channelManager;
        this.guildService = guildService;
        this.logger = logger;
    }

    public async Task ExecuteActivityAsync(string activityId)
    {
        logger.Debug("Executing activity with id {Id}", activityId);
        var activity = await mediator.Send(new GetActivityQuery(activityId));
        if (activity is null)
        {
            logger.Warning("No activity found, aborting execution");
            return;
        }

        switch (activity.Type)
        {
            case ActivityType.Unknown:
                logger.Information("Executed activity!");
                break;
            case ActivityType.SearchDungeon:
                try
                {
                    await ExecuteSearchDungeon(activity);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to execute SearchDungeon");
                }

                break;
            default:
                logger.Warning("Activity is not handled, {Name}", activity.Type);
                break;
        }

        await mediator.Send(new DeleteActivityCommand(activityId));
        logger.Information("Successfully executed and removed Activity {Name} after {Duration}", activity.Type,
            activity.Duration);
    }

    private async Task ExecuteSearchDungeon(Activity activity)
    {
        var threadId = await channelManager.CreateDungeonThreadAsync(activity.Data.GuildId, "Dungeon");

        var addDungeonResult = await guildService.AddDungeonToGuildAsync(activity.Data.GuildId, threadId,
            activity.Data.PlayerLevel, CancellationToken.None);

        if (!addDungeonResult.WasSuccessful)
        {
            logger.Warning("Failed to add dungeon from activity {Id}, deleting thread", activity.ID);
            await channelManager.DeleteDungeonThreadAsync(threadId);
        }

        await channelManager.UpdateDungeonThreadNameAsync(threadId, addDungeonResult.Value.Name);
        logger.Debug("Created Dungeon {Name} with Thread {Channel}", addDungeonResult.Value.Name, threadId);
    }
}