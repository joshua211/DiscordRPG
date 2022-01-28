using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.DomainServices;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Commands;

namespace DiscordRPG.Domain.Entities.Character.Commands;

public class UseItemCommand : Command<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public UseItemCommand(GuildId aggregateId, CharacterId entityId, ItemId itemId, TransactionContext context) :
        base(aggregateId)
    {
        EntityId = entityId;
        ItemId = itemId;
        Context = context;
    }

    public ItemId ItemId { get; private set; }
    public TransactionContext Context { get; private set; }
    public CharacterId EntityId { get; }
}

public class UseItemCommandHandler : CommandHandler<GuildAggregate, GuildId, UseItemCommand>
{
    private readonly IUseItemService useItemService;

    public UseItemCommandHandler(IUseItemService useItemService)
    {
        this.useItemService = useItemService;
    }

    public override Task ExecuteAsync(GuildAggregate aggregate, UseItemCommand command,
        CancellationToken cancellationToken)
    {
        useItemService.UseItem(aggregate, command.EntityId, command.ItemId, command.Context);
        return Task.CompletedTask;
    }
}