using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class UnequipTitleCommand : Command<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public UnequipTitleCommand(GuildId aggregateId, TitleId titleId, CharacterId entityId, TransactionContext context) :
        base(aggregateId)
    {
        TitleId = titleId;
        EntityId = entityId;
        Context = context;
    }

    public TitleId TitleId { get; private set; }
    public TransactionContext Context { get; private set; }
    public CharacterId EntityId { get; }
}

public class UnequipTitleCommandHandler : CommandHandler<GuildAggregate, GuildId, UnequipTitleCommand>
{
    public override Task ExecuteAsync(GuildAggregate aggregate, UnequipTitleCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UnequipTitle(command.EntityId, command.TitleId, command.Context);
        return Task.CompletedTask;
    }
}