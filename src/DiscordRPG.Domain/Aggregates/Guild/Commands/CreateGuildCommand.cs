using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Guild.Commands;

public class CreateGuildCommand : Command<GuildAggregate, GuildId>
{
    public CreateGuildCommand(GuildId aggregateId, GuildName name, ChannelId guildHallId,
        ChannelId dungeonHallId, ChannelId innId, TransactionContext context) : base(aggregateId)
    {
        Name = name;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        InnId = innId;
        Context = context;
    }

    public GuildName Name { get; private set; }
    public ChannelId GuildHallId { get; private set; }
    public ChannelId DungeonHallId { get; private set; }
    public ChannelId InnId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class CreateGuildCommandHandler : CommandHandler<GuildAggregate, GuildId, CreateGuildCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CreateGuildCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.AggregateId, command.Name, command.GuildHallId, command.DungeonHallId, command.InnId,
            command.Context);
        return Task.CompletedTask;
    }
}