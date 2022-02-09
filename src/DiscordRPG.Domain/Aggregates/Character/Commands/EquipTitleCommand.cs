using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class EquipTitleCommand : Command<CharacterAggregate, CharacterId>
{
    public EquipTitleCommand(CharacterId aggregateId, TitleId titleId, TransactionContext context) :
        base(aggregateId)
    {
        TitleId = titleId;
        Context = context;
    }

    public TitleId TitleId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class EquipTitleCommandHandler : CommandHandler<CharacterAggregate, CharacterId, EquipTitleCommand>
{
    public override Task ExecuteAsync(CharacterAggregate aggregate, EquipTitleCommand command,
        CancellationToken cancellationToken)
    {
        //aggregate.EquipTitle(command.TitleId, command.Context);
        return Task.CompletedTask;
    }
}