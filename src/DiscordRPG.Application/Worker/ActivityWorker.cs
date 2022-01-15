using Discord;
using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Activity;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using ActivityType = DiscordRPG.Domain.Entities.Activity.Enums.ActivityType;

namespace DiscordRPG.Application.Worker;

public class ActivityWorker
{
    private readonly IActivityService activityService;
    private readonly IChannelManager channelManager;
    private readonly ICharacterService characterService;
    private readonly IDungeonService dungeonService;
    private readonly ILogger logger;
    private readonly RarityGenerator rarityGenerator;

    public ActivityWorker(IChannelManager channelManager, IDungeonService dungeonService,
        ILogger logger, IActivityService activityService, ICharacterService characterService,
        RarityGenerator rarityGenerator)
    {
        this.channelManager = channelManager;
        this.dungeonService = dungeonService;
        this.logger = logger.WithContext(GetType());
        this.activityService = activityService;
        this.characterService = characterService;
        this.rarityGenerator = rarityGenerator;
    }

    public async Task ExecuteActivityAsync(ActivityId activityId)
    {
        var context = TransactionContext.New();
        var wasSuccess = true;
        logger.Context(context).Information("Executing activity with id {Id}", activityId);

        var activityResult = await activityService.GetActivityAsync(activityId, context);
        if (activityResult.Value is null)
        {
            logger.Context(context).Warning("No activity found, aborting execution");
            return;
        }

        var activity = activityResult.Value;
        try
        {
            switch (activity.Type)
            {
                case ActivityType.Unknown:
                    logger.Context(context).Information("Executed activity!");
                    break;
                case ActivityType.SearchDungeon:
                    await ExecuteSearchDungeon(activity, context);
                    break;
                case ActivityType.Dungeon:
                    await ExecuteEnterDungeon(activity, context);
                    break;
                case ActivityType.Rest:
                    await ExecuteRest(activity, context);
                    break;
                default:
                    logger.Context(context).Warning("Activity is not handled, {Name}", activity.Type);
                    break;
            }
        }
        catch (Exception e)
        {
            wasSuccess = false;
            logger.Context(context).Error(e, "Failed to execute {Name}", activity.GetType().Name);
        }

        await activityService.CompleteActivityAsync(new GuildId(activity.GuildId), new ActivityId(activity.Id),
            context);

        var word = wasSuccess ? "Successfully" : "Unsuccessfully";
        logger.Context(context).Information("{Word} executed and removed Activity {Name} after {Duration} minutes",
            word,
            activity.Type,
            (int) activity.Duration);
    }

    private async Task ExecuteRest(ActivityReadModel activity, TransactionContext context)
    {
        var result = await characterService.RestoreWoundsFromRestAsync(new GuildId(activity.GuildId),
            new CharacterId(activity.CharacterId), activity.Duration, context);
        if (!result.WasSuccessful)
        {
            logger.Context(context).Error("Failed resting");
            return;
        }

        var embed = new EmbedBuilder().WithTitle("Rest complete")
            .WithDescription($"<@{activity.ActivityData.UserId}> you are done resting!").Build();

        await channelManager.SendToInn(activity.ActivityData.GuildId, String.Empty, context, embed);
    }

    private async Task ExecuteEnterDungeon(ActivityReadModel activity, TransactionContext context)
    {
        var executionResult =
            await dungeonService.CalculateDungeonAdventureResultAsync(new GuildId(activity.GuildId),
                new DungeonId(activity.ActivityData.DungeonId.Value), new CharacterId(activity.CharacterId),
                activity.Duration, context);
    }

    private async Task ExecuteSearchDungeon(ActivityReadModel activity, TransactionContext context)
    {
        var charResult = await characterService.GetCharacterAsync(new CharacterId(activity.CharacterId), context);
        if (!charResult.WasSuccessful)
        {
            logger.Context(context).Error("No character with ID {Id} found, cant execute SearchDungeon",
                activity.CharacterId);
            return;
        }

        var character = charResult.Value;

        var createDungeonResult =
            await dungeonService.CreateDungeonAsync(new GuildId(activity.ActivityData.GuildId.Value),
                new CharacterId(character.Id), character.Level.CurrentLevel, character.Luck, activity.Duration,
                context);

        if (!createDungeonResult.WasSuccessful)
        {
            logger.Context(context).Warning("Failed to add dungeon from activity {Id}", activity.Id);

            return;
        }

        logger.Context(context).Debug("Created Dungeon");
    }
}