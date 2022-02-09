using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.DomainServices;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Aggregates.Character.Commands;

public class UseItemCommand : Command<CharacterAggregate, CharacterId>
{
    public UseItemCommand(CharacterId aggregateId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
}

public class UseItemCommandHandler : CommandHandler<CharacterAggregate, CharacterId, UseItemCommand>
{
    private readonly IUseItemService useItemService;

    public UseItemCommandHandler(IUseItemService useItemService)
    {
        this.useItemService = useItemService;
    }

    public override Task ExecuteAsync(CharacterAggregate aggregate, UseItemCommand command,
        CancellationToken cancellationToken)
    {
        //useItemService.UseItem(aggregate, command.EntityId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}