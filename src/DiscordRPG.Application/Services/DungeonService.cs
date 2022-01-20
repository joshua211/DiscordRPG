using DiscordRPG.Application.Generators;
using DiscordRPG.Application.Interfaces;
using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.Commands;
using DiscordRPG.Domain.Entities.Activity.Enums;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using DiscordRPG.Domain.Entities.Dungeon.Commands;
using EventFlow;
using EventFlow.Queries;

namespace DiscordRPG.Application.Services;

public class DungeonService : IDungeonService
{
    private readonly ICommandBus bus;
    private readonly IChannelManager channelManager;
    private readonly DungeonGenerator dungeonGenerator;
    private readonly ILogger logger;
    private readonly IQueryProcessor processor;

    public DungeonService(IQueryProcessor processor, ICommandBus bus, ILogger logger, DungeonGenerator dungeonGenerator,
        IChannelManager channelManager)
    {
        this.processor = processor;
        this.bus = bus;
        this.logger = logger;
        this.dungeonGenerator = dungeonGenerator;
        this.channelManager = channelManager;
    }

    public async Task<Result> CreateDungeonAsync(GuildId guildId, CharacterId character, uint charLevel, int charLuck,
        ActivityDuration duration,
        TransactionContext context, CancellationToken token = default)
    {
        logger.Context(context).Information("Creating dungeon");
        var id = await channelManager.CreateDungeonThreadAsync(guildId, "Dungeon", context);
        var dungeon = dungeonGenerator.GenerateRandomDungeon(new DungeonId(id.Value), charLevel, charLuck, duration);
        var cmd = new AddDungeonCommand(guildId, dungeon, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to add dungeon");
            return Result.Failure("Failed to add dungeon");
        }

        await channelManager.UpdateDungeonThreadNameAsync(id, dungeon.Name.Value, context);

        return Result.Success();
    }

    public async Task<Result<DungeonReadModel>> GetDungeonAsync(DungeonId dungeonId, TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Querying dungeon with Id {Id}", dungeonId.Value);
        var query = new GetDungeonQuery(dungeonId);
        var result = await processor.ProcessAsync(query, token);

        return Result<DungeonReadModel>.Success(result);
    }

    public async Task<Result> CalculateDungeonAdventureResultAsync(GuildId guildId, DungeonId dungeonId,
        CharacterId characterId,
        ActivityDuration activityDuration, TransactionContext context, CancellationToken token = default)
    {
        logger.Context(context).Information("Calculating adventure Result for Character {CharId} with Dungeon {DungId}",
            characterId.Value, dungeonId.Value);
        var cmd = new CalculateAdventureResultCommand(guildId, dungeonId, characterId, activityDuration, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to calculate adventure result");
            return Result.Failure("Failed to calculate");
        }

        return Result.Success();
    }

    public async Task<Result> DecreaseExplorationsAsync(GuildId guildId, DungeonId dungeonId,
        TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Reducing explorations for Dungeon with Id {Id} from Guild {Guild}",
            dungeonId.Value, guildId.Value);
        var cmd = new DecreaseDungeonExplorationsCommand(guildId, dungeonId, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to decrease explorations for Dungeon with id {Id}", dungeonId.Value);
            return Result.Failure("Failed to decrease explorations");
        }

        return Result.Success();
    }

    public async Task<Result<IEnumerable<DungeonReadModel>>> GetAllDungeonsAsync(GuildId guildId,
        TransactionContext context, CancellationToken token = default)
    {
        var query = new GetAllDungeonsQuery(guildId);
        var result = await processor.ProcessAsync(query, token);

        return Result<IEnumerable<DungeonReadModel>>.Success(result);
    }

    public async Task<Result> DeleteDungeonAsync(GuildId guildId, DungeonId dungeonId, TransactionContext context,
        CancellationToken token = default)
    {
        logger.Context(context).Information("Deleting Dungeon with Id {Id} from Guild {Guild}", dungeonId.Value,
            guildId.Value);
        var cmd = new RemoveDungeonCommand(guildId, dungeonId, context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to delete dungeon with id {Id}", dungeonId.Value);
            return Result.Failure("Failed to delete dungeon");
        }

        return Result.Success();
    }
}