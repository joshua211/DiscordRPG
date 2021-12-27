using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Queries;
using DiscordRPG.Common;
using DiscordRPG.Core.Commands.Guilds;
using MediatR;

namespace DiscordRPG.Application.Services;

public class GuildService : ApplicationService, IGuildService
{
    public GuildService(IMediator mediator, ILogger logger) : base(mediator, logger)
    {
    }

    public async Task<Result<Guild>> GetGuildAsync(Identity identity, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetGuildQuery(identity);
            var result = await ProcessAsync(ctx, query, cancellationToken);
            if (result is null)
            {
                TransactionWarning(ctx, "No guild found for id {Identity}", identity);
                return Result<Guild>.Failure("No guild found");
            }

            return Result<Guild>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<Guild>.Failure(e.Message);
        }
    }

    public async Task<Result<Guild>> GetGuildWithDiscordIdAsync(DiscordId serverId,
        TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetGuildByServerIdQuery(serverId);
            var result = await ProcessAsync(ctx, query, cancellationToken);

            if (result is null)
            {
                TransactionWarning(ctx, "No Guild found with id {ID}", serverId);

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

    public async Task<Result<Guild>> CreateGuildAsync(DiscordId serverId, string guildName, DiscordId guildHallId,
        DiscordId dungeonHallId, TransactionContext parentContext = null,
        CancellationToken token = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var guild = new Guild(serverId, guildName, guildHallId, dungeonHallId);
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

    public async Task<Result> DeleteGuildAsync(DiscordId serverId, TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var cmd = new DeleteGuildCommand(serverId);
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

    public async Task<Result<IEnumerable<Guild>>> GetAllGuildsAsync(TransactionContext parentContext = null,
        CancellationToken cancellationToken = default)
    {
        using var ctx = TransactionBegin(parentContext);
        try
        {
            var query = new GetAllGuildsQuery();
            var result = await ProcessAsync(ctx, query, cancellationToken);

            return Result<IEnumerable<Guild>>.Success(result);
        }
        catch (Exception e)
        {
            TransactionError(ctx, e);
            return Result<IEnumerable<Guild>>.Failure(e.Message);
        }
    }
}