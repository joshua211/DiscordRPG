using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class UnequipTitleCommand : Command<CharacterAggregate, CharacterId>
{
    public UnequipTitleCommand(CharacterId aggregateId, TitleId titleId, TransactionContext context) :
        base(aggregateId)
    {
        TitleId = titleId;
        Context = context;
    }

    public TitleId TitleId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class UnequipTitleCommandHandler : CommandHandler<CharacterAggregate, CharacterId, UnequipTitleCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, UnequipTitleCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.UnequipTitle(command.EntityId, command.TitleId, command.Context);
        return Task.CompletedTask;
    }
}