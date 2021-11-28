using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Dungeons;
using MediatR;

namespace DiscordRPG.Application.Services;

public class DungeonService : ApplicationService, IDungeonService
{
    private readonly ICharacterService characterService;
    private readonly IGuildService guildService;

    public DungeonService(IMediator mediator, ILogger logger, IGuildService guildService,
        ICharacterService characterService) : base(mediator, logger)
    {
        this.guildService = guildService;
        this.characterService = characterService;
    }

    public async Task<Result<Dungeon>> CreateDungeonAsync(DiscordId serverId, DiscordId threadId, uint charLevel,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var guildResult = await guildService.GetGuildWithDiscordIdAsync(serverId, ctx, token);
            if (!guildResult.WasSuccessful)
            {
                TransactionWarning(ctx, "No guild found to add the dungeon");

                return Result<Dungeon>.Failure("No guild found");
            }


            //TODO generate dungeon
            var dungeon = new Dungeon(serverId, threadId, charLevel, Rarity.Common, "Some new name", 10);
            var cmd = new CreateDungeonCommand(dungeon);

            var result = await PublishAsync(ctx, cmd, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx,
                    "Failed to create Dungeon with channelId: {ChannelId}, for guild: {GuildId} because: {Reason}",
                    threadId, serverId, result.ErrorMessage);

                return Result<Dungeon>.Failure("Failed to create dungeon");
            }

            return Result<Dungeon>.Success(dungeon);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Dungeon>.Failure(e.Message);
        }
    }

    public async Task<Result<Dungeon>> GetDungeonFromChannelIdAsync(DiscordId channelId,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetDungeonByChannelIdQuery(channelId);
            var result = await ProcessAsync(ctx, query, token);
            if (result is null)
            {
                TransactionWarning(ctx, "No dungeon with channelId {Id} found", channelId);
                return Result<Dungeon>.Failure("No dungeon found");
            }

            return Result<Dungeon>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Dungeon>.Failure(e.Message);
        }
    }

    public async Task<Result> CalculateDungeonAdventureResultAsync(Identity charId, DiscordId threadId,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var charResult = await characterService.GetCharacterAsync(charId, ctx, token: token);
            if (!charResult.WasSuccessful)
            {
                TransactionWarning(ctx, "No character with ID {Id} found to execute dungeon search", charId);
                return Result.Failure(charResult.ErrorMessage);
            }

            var dungeonResult = await GetDungeonFromChannelIdAsync(threadId, ctx, token: token);
            if (!dungeonResult.WasSuccessful)
            {
                TransactionWarning(ctx, "No dungeon with channelID {Id} found to execute dungeon search", threadId);
                return Result.Failure(charResult.ErrorMessage);
            }

            var command = new CalculateAdventureResultCommand(charResult.Value, dungeonResult.Value);
            var result = await PublishAsync(ctx, command, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to calculate dungeon result: {Reason}", result.ErrorMessage);
                return Result.Failure("Failed to calculate result");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result> DecreaseExplorationsAsync(Dungeon dungeon, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var command = new DecreaseExplorationCommand(dungeon);
            var result = await PublishAsync(ctx, command, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to decrease dungeon explorations: {Reason}", result.ErrorMessage);
                return Result.Failure("Failed to decrease explorations");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<IEnumerable<Dungeon>>> GetAllDungeonsAsync(TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetAllDungeonsQuery();
            var result = await ProcessAsync(ctx, query, token);

            return Result<IEnumerable<Dungeon>>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<IEnumerable<Dungeon>>.Failure(e.Message);
        }
    }

    public async Task<Result> DeleteDungeonAsync(Dungeon dungeon, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var command = new DeleteDungeonCommand(dungeon.ID);
            var result = await PublishAsync(ctx, command, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to delete dungeon: {Reason}", result.ErrorMessage);
                return Result.Failure("Failed to Delete Dungeon");
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }
}