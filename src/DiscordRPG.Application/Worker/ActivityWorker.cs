using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.Commands.Activities;
using MediatR;

namespace DiscordRPG.Application.Worker;

public class ActivityWorker
{
    private readonly IActivityService activityService;
    private readonly IChannelManager channelManager;
    private readonly ICharacterService characterService;
    private readonly IDungeonService dungeonService;
    private readonly ILogger logger;
    private readonly IMediator mediator;

    public ActivityWorker(IMediator mediator, IChannelManager channelManager, IDungeonService dungeonService,
        ILogger logger, IActivityService activityService, ICharacterService characterService)
    {
        this.mediator = mediator;
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger.WithContext(GetType());
        this.activityService = activityService;
        this.characterService = characterService;
    }

    public async Task ExecuteActivityAsync(string activityId)
    {
        var wasSuccess = true;
        logger.Here().Information("Executing activity with id {Id}", activityId);

        var activityResult = await activityService.GetActivityAsync(activityId);
        if (!activityResult.WasSuccessful)
        {
            logger.Here().Warning("No activity found, aborting execution");
            return;
        }

        var activity = activityResult.Value;
        try
        {
            switch (activity.Type)
            {
                case ActivityType.Unknown:
                    logger.Here().Information("Executed activity!");
                    break;
                case ActivityType.SearchDungeon:
                    await ExecuteSearchDungeon(activity);
                    break;
                case ActivityType.Dungeon:
                    await ExecuteEnterDungeon(activity);
                    break;
                default:
                    logger.Here().Warning("Activity is not handled, {Name}", activity.Type);
                    break;
            }
        }
        catch (Exception e)
        {
            wasSuccess = false;
            logger.Here().Error(e, "Failed to execute {Name}", activity.GetType().Name);
        }

        await mediator.Send(new DeleteActivityCommand(activityId));

        var word = wasSuccess ? "Successfully" : "Unsuccessfully";
        logger.Here().Information("{Word} executed and removed Activity {Name} after {Duration}", word, activity.Type,
            activity.Duration);
    }

    private async Task ExecuteEnterDungeon(Activity activity)
    {
        var executionResult =
            await dungeonService.GetDungeonAdventureResultAsync(activity.CharId, activity.Data.ThreadId);
        if (!executionResult.WasSuccessful)
        {
            logger.Here().Warning("Failed to get execution result for dungeon search");
            return;
        }

        logger.Here().Information("Sustained {Count} wound(s)", executionResult.Value.Wounds.Count);

        /*var woundResult  = await characterService.ApplyWoundsAsync(executionResult.Value.Wounds);
        if (!woundResult.WasSuccessful)
        {
            logger.Here().Warning("Failed to apply wounds, stopping execution");
            return;
        }*/

        //var lootResult = characterService.AddItemsAsync()
        //get character
        //get dungeon

        //result = GetDungeonResult(dungeon, character)
        //ApplyWoundsToCharacter(result.Wounds)
        //AddItemsToInventory(result.Loot)
    }

    private async Task ExecuteSearchDungeon(Activity activity)
    {
        var threadId = await channelManager.CreateDungeonThreadAsync(activity.Data.ServerId, "Dungeon");

        var createDungeonResult = await dungeonService.CreateDungeonAsync(activity.Data.ServerId, threadId,
            activity.Data.PlayerLevel);

        if (!createDungeonResult.WasSuccessful)
        {
            logger.Here().Warning("Failed to add dungeon from activity {Id}, deleting thread", activity.ID);
            await channelManager.DeleteDungeonThreadAsync(threadId);

            return;
        }

        await channelManager.UpdateDungeonThreadNameAsync(threadId, createDungeonResult.Value.Name);
        logger.Here().Debug("Created Dungeon {Name} with Thread {Channel}", createDungeonResult.Value.Name, threadId);
    }
}