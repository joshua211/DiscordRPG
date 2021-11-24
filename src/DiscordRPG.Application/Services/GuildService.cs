using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Guilds;
using MediatR;
using Serilog;

namespace DiscordRPG.Application.Services;

public class GuildService : ApplicationService, IGuildService
{
    public GuildService(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Result<Guild>> GetGuildAsync(ulong guildId, CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var query = new GetGuildQuery(guildId);
            var result = await ProcessAsync(ctx, query, cancellationToken);

            if (result is null)
            {
                TransactionWarning(ctx, "No Guild found with id {ID}", true, guildId);

                return Result<Guild>.Failure("No Guild found");
            }

            return Result<Guild>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Guild>.Failure(e.Message);
        }
    }

    public async Task<Result<Guild>> CreateGuildAsync(ulong guildId, string guildName, ulong guildHallId,
        ulong dungeonHallId,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var guild = new Guild(guildId, guildName, guildHallId, dungeonHallId, new List<ulong>(),
                new List<Dungeon>());
            var cmd = new CreateGuildCommand(guild);

            var result = await PublishAsync(ctx, cmd, token);
            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to create guild {@Guild}", guild);
                return Result<Guild>.Failure("Failed to create Guild");
            }

            return Result<Guild>.Success(guild);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Guild>.Failure(e.Message);
        }
    }

    public async Task<Result> DeleteGuildAsync(ulong id, CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var cmd = new DeleteGuildCommand(id);
            var result = await PublishAsync(ctx, cmd, cancellationToken);

            if (!result.WasSuccessful)
            {
                TransactionError(ctx, "Failed to delete guild: {Reason}", result.ErrorMessage);
                return Result.Failure("Failed to delete guild: " + result.ErrorMessage);
            }

            return Result.Success();
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result.Failure(e.Message);
        }
    }

    public async Task<Result<Dungeon>> AddDungeonToGuildAsync(ulong guildId, ulong threadId, uint charLevel,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin();
        try
        {
            var guildResult = await GetGuildAsync(guildId, token);
            if (!guildResult.WasSuccessful)
            {
                TransactionWarning(ctx, "No guild found to add the dungeon");

                return Result<Dungeon>.Failure("No guild found");
            }


            //TODO generate dungeon
            var dungeon = new Dungeon(threadId, charLevel, Rarity.Common, "SOme new name");
            var cmd = new AddDungeonCommand(dungeon, guildResult.Value);

            var result = await PublishAsync(ctx, cmd, token);
            if (!result.WasSuccessful)
            {
                TransactionWarning(ctx, "Failed to create Dungeon with channelId: {ChannelId}, for guild: {GuildId}",
                    true, threadId, guildId);

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
}