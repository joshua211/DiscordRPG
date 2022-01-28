using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class ChangeWoundsCommand : Command<GuildAggregate, GuildId>
{
    public ChangeWoundsCommand(GuildId aggregateId, CharacterId characterId, List<Wound> newWounds,
        TransactionContext context) : base(aggregateId)
    {
        CharacterId = characterId;
        NewWounds = newWounds;
        Context = context;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Wound> NewWounds { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class ChangeWoundsCommandHandler : CommandHandler<GuildAggregate, GuildId, ChangeWoundsCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, ChangeWoundsCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.ChangeWounds(command.CharacterId, command.NewWounds, command.Context);
        return Task.CompletedTask;
    }
}