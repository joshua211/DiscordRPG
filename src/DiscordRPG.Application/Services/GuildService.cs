using DiscordRPG.Application.Interfaces.Services;
using DiscordRPG.Application.Models;
using DiscordRPG.Application.Queries;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.Commands;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow;
using EventFlow.Queries;

namespace DiscordRPG.Application.Services;

public class GuildService : IGuildService
{
    private readonly ICommandBus bus;
    private readonly ILogger logger;
    private readonly IQueryProcessor processor;

    public GuildService(IQueryProcessor processor, ILogger logger, ICommandBus bus)
    {
        this.processor = processor;
        this.bus = bus;
        this.logger = logger.WithContext(GetType());
    }

    public async Task<Result<GuildReadModel>> GetGuildAsync(GuildId identity, TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Querying guild with id {ID}", identity.Value);
        var query = new GetGuildQuery(identity);
        var result = await processor.ProcessAsync(query, cancellationToken);
        logger.Context(context).Information("Found guild: {Guild}", result?.GuildName.Value ?? "null");

        return Result<GuildReadModel>.Success(result);
    }

    public async Task<Result> CreateGuildAsync(string serverId, string guildName, string guildHallId,
        string dungeonHallId, string innChannel,
        TransactionContext context, CancellationToken token = default)
    {
        logger.Context(context).Information("Creating guild with Id {Id}", serverId);
        var cmd = new CreateGuildCommand(new GuildId(serverId), new GuildName(guildName), new ChannelId(guildHallId),
            new ChannelId(dungeonHallId), new ChannelId(innChannel), context);
        var result = await bus.PublishAsync(cmd, token);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to create guild");
            return Result.Failure("Failed to create guild");
        }

        logger.Context(context).Information("Created Guild");

        return Result.Success();
    }

    public async Task<Result> DeleteGuildAsync(GuildId id, TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Deleting guild with Id {Id}", id.Value);
        var cmd = new DeleteGuildCommand(id, context);
        var result = await bus.PublishAsync(cmd, cancellationToken);

        if (!result.IsSuccess)
        {
            logger.Context(context).Error("Failed to delete guild");
            return Result.Failure("Failed to delete guild");
        }

        logger.Context(context).Information("Deleted Guild");

        return Result.Success();
    }

    public async Task<Result<IEnumerable<GuildReadModel>>> GetAllGuildsAsync(TransactionContext context,
        CancellationToken cancellationToken = default)
    {
        logger.Context(context).Information("Querying all guilds");
        var query = new GetAllGuildsQuery();
        var result = await processor.ProcessAsync(query, cancellationToken);
        logger.Context(context).Information("Found {Count} guild", result.Count());

        return Result<IEnumerable<GuildReadModel>>.Success(result);
    }
}