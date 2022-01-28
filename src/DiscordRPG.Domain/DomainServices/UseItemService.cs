using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Domain.DomainServices;

public class UseItemService : IUseItemService
{
    private readonly IHealthPotionCalculator healthPotionCalculator;
    private readonly IWoundReducer woundReducer;

    public UseItemService(IHealthPotionCalculator healthPotionCalculator, IWoundReducer woundReducer)
    {
        this.healthPotionCalculator = healthPotionCalculator;
        this.woundReducer = woundReducer;
    }

    public void UseItem(GuildAggregate aggregate, CharacterId entityId, ItemId itemId,
        TransactionContext transactionContext)
    {
        var character = aggregate.Characters.First(c => Equals(c.Id, entityId));
        var item = character.Inventory.First(i => i.Id.Equals(itemId));

        switch (item.UsageEffect)
        {
            case UsageEffect.RestoreHealth:
            {
                var amountToHeal = healthPotionCalculator.CalculateHealAmount(item.Rarity, item.Level);
                var newWounds = woundReducer.ReduceDamageFromWounds(character.Wounds, amountToHeal);

                aggregate.ChangeWounds(entityId, newWounds.ToList(), transactionContext);
                break;
            }
        }

        var newInventory = character.Inventory;
        if (item.Amount > 1)
            item.IncreaseAmount(-1);
        else
            newInventory.Remove(item);

        aggregate.ChangeCharacterInventory(entityId, newInventory, transactionContext);
    }
}

public interface IUseItemService
{
    void UseItem(GuildAggregate aggregate, CharacterId entityId, ItemId itemId, TransactionContext transactionContext);
}