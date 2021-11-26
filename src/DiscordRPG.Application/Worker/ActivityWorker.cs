using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.Commands.Activities;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Worker;

public class ActivityWorker
{
    private readonly IActivityService activityService;
    private readonly IChannelManager channelManager;
    private readonly IDungeonService dungeonService;
    private readonly ILogger logger;
    private readonly IMediator mediator;

    public ActivityWorker(IMediator mediator, IChannelManager channelManager, IDungeonService dungeonService,
        ILogger logger, IActivityService activityService)
    {
        this.mediator = mediator;
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger;
        this.activityService = activityService;
    }

    public async Task ExecuteActivityAsync(string activityId)
    {
        var wasSuccess = true;
        logger.Debug("Executing activity with id {Id}", activityId);

        var activityResult = await activityService.GetActivityAsync(activityId);
        if (!activityResult.WasSuccessful)
        {
            logger.Warning("No activity found, aborting execution");
            return;
        }

        var activity = activityResult.Value;
        try
        {
            switch (activity.Type)
            {
                case ActivityType.Unknown:
                    logger.Information("Executed activity!");
                    break;
                case ActivityType.SearchDungeon:
                    await ExecuteSearchDungeon(activity);
                    break;
                case ActivityType.Dungeon:
                    await ExecuteEnterDungeon(activity);
                    break;
                default:
                    logger.Warning("Activity is not handled, {Name}", activity.Type);
                    break;
            }
        }
        catch (Exception e)
        {
            wasSuccess = false;
            logger.Error(e, "Failed to execute {Name}", activity.GetType().Name);
        }

        await mediator.Send(new DeleteActivityCommand(activityId));
        var word = wasSuccess ? "Successfully" : "Unsuccessfully";
        logger.Information("{Word} executed and removed Activity {Name} after {Duration}", word, activity.Type,
            activity.Duration);
    }

    private async Task ExecuteEnterDungeon(Activity activity)
    {
        logger.Information("{Name} has been completed!", activity.Data.ThreadId);
    }

    private async Task ExecuteSearchDungeon(Activity activity)
    {
        var threadId = await channelManager.CreateDungeonThreadAsync(activity.Data.ServerId, "Dungeon");

        var createDungeonResult = await dungeonService.CreateDungeonAsync(activity.Data.ServerId, threadId,
            activity.Data.PlayerLevel);

        if (!createDungeonResult.WasSuccessful)
        {
            logger.Warning("Failed to add dungeon from activity {Id}, deleting thread", activity.ID);
            await channelManager.DeleteDungeonThreadAsync(threadId);
        }

        await channelManager.UpdateDungeonThreadNameAsync(threadId, createDungeonResult.Value.Name);
        logger.Debug("Created Dungeon {Name} with Thread {Channel}", createDungeonResult.Value.Name, threadId);
    }
}