using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Core.Commands.Activities;
using DiscordRPG.Core.DomainServices.Generators;
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
    private readonly IRarityGenerator rarityGenerator;

    public ActivityWorker(IMediator mediator, IChannelManager channelManager, IDungeonService dungeonService,
        ILogger logger, IActivityService activityService, ICharacterService characterService,
        IRarityGenerator rarityGenerator)
    {
        this.mediator = mediator;
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger.WithContext(GetType());
        this.activityService = activityService;
        this.characterService = characterService;
        this.rarityGenerator = rarityGenerator;
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
        logger.Here().Information("{Word} executed and removed Activity {Name} after {Duration} minutes", word,
            activity.Type,
            (int) activity.Duration);
    }

    private async Task ExecuteEnterDungeon(Activity activity)
    {
        var executionResult =
            await dungeonService.CalculateDungeonAdventureResultAsync(activity.CharId, activity.Data.ThreadId,
                activity.Duration);
    }

    private async Task ExecuteSearchDungeon(Activity activity)
    {
        var charResult = await characterService.GetCharacterAsync(activity.CharId);
        if (!charResult.WasSuccessful)
        {
            logger.Here().Error("No character with ID {Id} found, cant execute SearchDungeon", activity.CharId.Value);
            return;
        }

        var character = charResult.Value;
        var threadId = await channelManager.CreateDungeonThreadAsync(activity.Data.ServerId, "Dungeon");

        var rarity = rarityGenerator.GenerateRarityFromActivityDuration(activity.Duration);

        var createDungeonResult = await dungeonService.CreateDungeonAsync(activity.Data.ServerId, threadId,
            character, rarity);

        if (!createDungeonResult.WasSuccessful)
        {
            logger.Here().Warning("Failed to add dungeon from activity {Id}, deleting thread", activity.ID);
            await channelManager.DeleteDungeonThreadAsync(threadId);

            return;
        }

        logger.Here().Debug("Created Dungeon {Name} with Thread {Channel}", createDungeonResult.Value.Name, threadId);
    }
}