using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Dungeons;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class DungeonService : ApplicationService, IDungeonService
{
    private readonly IGuildService guildService;

    public DungeonService(IMediator mediator, ILogger logger, IGuildService guildService) : base(mediator, logger)
    {
        this.guildService = guildService;
    }

    public async Task<Result<Dungeon>> CreateDungeonAsync(ulong guildId, ulong threadId, uint charLevel,
        TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var guildResult = await guildService.GetGuildAsync(guildId, ctx, token);
            if (!guildResult.WasSuccessful)
            {
                TransactionWarning(ctx, "No guild found to add the dungeon");

                return Result<Dungeon>.Failure("No guild found");
            }


            //TODO generate dungeon
            var dungeon = new Dungeon(guildId, threadId, charLevel, Rarity.Common, "Some new name");
            var cmd = new CreateDungeonCommand(dungeon);

            var result = await PublishAsync(ctx, cmd, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx,
                    "Failed to create Dungeon with channelId: {ChannelId}, for guild: {GuildId} because: {Reason}",
                    threadId, guildId, result.ErrorMessage);

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

    public async Task<Result<Dungeon>> GetDungeonAsync(ulong channelId, TransactionContext parentContext = null,
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
}