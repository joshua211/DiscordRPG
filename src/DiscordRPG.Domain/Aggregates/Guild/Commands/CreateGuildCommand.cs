using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Guild.Commands;

public class CreateGuildCommand : Command<GuildAggregate, GuildId>
{
    public CreateGuildCommand(GuildId aggregateId, GuildId id, GuildName name, ChannelId guildHallId,
        ChannelId dungeonHallId, ChannelId innId) : base(aggregateId)
    {
        Id = id;
        Name = name;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        InnId = innId;
    }

    public GuildId Id { get; private set; }
    public GuildName Name { get; private set; }
    public ChannelId GuildHallId { get; private set; }
    public ChannelId DungeonHallId { get; private set; }
    public ChannelId InnId { get; private set; }
}

public class CreateGuildCommandHandler : CommandHandler<GuildAggregate, GuildId, CreateGuildCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, CreateGuildCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.Id, command.Name, command.GuildHallId, command.DungeonHallId, command.InnId);
        return Task.CompletedTask;
    }
}