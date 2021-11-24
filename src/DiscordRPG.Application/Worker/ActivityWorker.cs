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
    private readonly IDungeonService dungeonService;
    private readonly ILogger logger;
    private readonly IMediator mediator;

    public ActivityWorker(IMediator mediator, IChannelManager channelManager, IDungeonService dungeonService,
        ILogger logger)
    {
        this.mediator = mediator;
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger;
    }

    public async Task ExecuteActivityAsync(string activityId)
    {
        var wasSuccess = true;
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
                    wasSuccess = false;
                    logger.Error(e, "Failed to execute SearchDungeon");
                }

                break;
            default:
                logger.Warning("Activity is not handled, {Name}", activity.Type);
                break;
        }

        await mediator.Send(new DeleteActivityCommand(activityId));
        var word = wasSuccess ? "Successfully" : "Unsuccessfully";
        logger.Information("{Word} executed and removed Activity {Name} after {Duration}", word, activity.Type,
            activity.Duration);
    }

    private async Task ExecuteSearchDungeon(Activity activity)
    {
        var threadId = await channelManager.CreateDungeonThreadAsync(activity.Data.GuildId, "Dungeon");

        var createDungeonResult = await dungeonService.CreateDungeonAsync(activity.Data.GuildId, threadId,
            activity.Data.PlayerLevel, CancellationToken.None);

        if (!createDungeonResult.WasSuccessful)
        {
            logger.Warning("Failed to add dungeon from activity {Id}, deleting thread", activity.ID);
            await channelManager.DeleteDungeonThreadAsync(threadId);
        }

        await channelManager.UpdateDungeonThreadNameAsync(threadId, createDungeonResult.Value.Name);
        logger.Debug("Created Dungeon {Name} with Thread {Channel}", createDungeonResult.Value.Name, threadId);
    }
}