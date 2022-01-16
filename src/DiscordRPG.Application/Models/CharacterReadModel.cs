﻿using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Entities.Character.Events;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class CharacterReadModel : IMongoDbReadModel,
    IAmReadModelFor<GuildAggregate, GuildId, CharacterCreated>,
    IAmReadModelFor<GuildAggregate, GuildId, CharacterDied>,
    IAmReadModelFor<GuildAggregate, GuildId, InventoryChanged>,
    IAmReadModelFor<GuildAggregate, GuildId, ItemBought>,
    IAmReadModelFor<GuildAggregate, GuildId, ItemEquipped>,
    IAmReadModelFor<GuildAggregate, GuildId, ItemUnequipped>,
    IAmReadModelFor<GuildAggregate, GuildId, LevelGained>,
    IAmReadModelFor<GuildAggregate, GuildId, WoundsChanged>
{
    public CharacterClass Class { get; set; }
    public CharacterRace Race { get; set; }
    public CharacterName Name { get; set; }
    public Level Level { get; set; }
    public Money Money { get; set; }
    public List<Item> Inventory { get; set; }
    public List<Wound> Wounds { get; set; }

    public Item? Weapon => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Weapon);
    public Item? Helmet => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Helmet);
    public Item? TorsoArmor => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Armor);
    public Item? Pants => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Pants);
    public Item? Amulet => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Amulet);
    public Item? Ring => Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == EquipmentPosition.Ring);

    public int Strength
    {
        get
        {
            var totalMod = Class.StrengthModifier + Race.StrengthModifier;
            var charAttr = (int) (Class.BaseStrength + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Inventory.Where(i => i.IsEquipped).Sum(i => i.Strength);

            return charAttr + equipAttr;
        }
    }

    public int Vitality
    {
        get
        {
            var totalMod = Class.VitalityModifier + Race.VitalityModifier;
            var charAttr = (int) (Class.BaseVitality + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Inventory.Where(i => i.IsEquipped).Sum(i => i.Vitality);

            return charAttr + equipAttr;
        }
    }

    public int Agility
    {
        get
        {
            var totalMod = Class.AgilityModifier + Race.AgilityModifier;
            var charAttr = (int) (Class.BaseAgility + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Inventory.Where(i => i.IsEquipped).Sum(i => i.Agility);

            return charAttr + equipAttr;
        }
    }

    public int Intelligence
    {
        get
        {
            var totalMod = Class.IntelligenceModifier + Race.IntelligenceModifier;
            var charAttr = (int) (Class.BaseIntelligence + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Inventory.Where(i => i.IsEquipped).Sum(i => i.Intelligence);

            return charAttr + equipAttr;
        }
    }

    public int Armor
    {
        get { return Inventory.Where(i => i.IsEquipped).Sum(i => i.Armor); }
    }

    public int MagicArmor
    {
        get { return Inventory.Where(i => i.IsEquipped).Sum(i => i.MagicArmor); }
    }

    public Damage TotalDamage
    {
        get
        {
            var weapon = Inventory.FirstOrDefault(i => i.IsEquipped && i.ItemType == ItemType.Weapon);
            if (weapon == null)
                return new Damage(DamageType.Physical, 1);

            var dmgFromAttributes = weapon.DamageAttribute switch
            {
                CharacterAttribute.Strength => Strength + (int) (Vitality * 0.1) + (int) (Agility * 0.1) +
                                               (int) (Intelligence * 0.1),
                CharacterAttribute.Vitality => Vitality + (int) (Strength * 0.1) + (int) (Agility * 0.1) +
                                               (int) (Intelligence * 0.1),
                CharacterAttribute.Agility => Agility + (int) (Vitality * 0.1) + (int) (Strength * 0.1) +
                                              (int) (Intelligence * 0.1),
                CharacterAttribute.Intelligence => Intelligence + (int) (Vitality * 0.1) + (int) (Agility * 0.1) +
                                                   (int) (Strength * 0.1),
                _ => 0
            };

            return new Damage(weapon.DamageType, dmgFromAttributes + weapon.DamageValue);
        }
    }

    public int MaxHealth => Vitality * 10;

    public int CurrentHealth => MaxHealth - Wounds.Sum(w => w.DamageValue);

    public int Luck
    {
        get
        {
            var totalMod = Class.StrengthModifier + Race.StrengthModifier;
            var charAttr = (int) (Class.BaseStrength + (1 * Level.CurrentLevel * totalMod));
            var equipAttr = Inventory.Where(i => i.IsEquipped).Sum(i => i.Strength);

            return charAttr + equipAttr;
        }
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, CharacterCreated> domainEvent)
    {
        var character = domainEvent.AggregateEvent.Character;
        Id = character.Id.Value;
        //Id = domainEvent.AggregateIdentity.Value;
        Name = character.Name;
        Class = character.Class;
        Race = character.Race;
        Level = character.CharacterLevel;
        Money = character.Money;
        Inventory = character.Inventory;
        Wounds = character.Wounds;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, CharacterDied> domainEvent)
    {
        context.MarkForDeletion();
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, InventoryChanged> domainEvent)
    {
        Inventory = domainEvent.AggregateEvent.NewInventory;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ItemBought> domainEvent)
    {
        Money = Money.Add(-domainEvent.AggregateEvent.Item.Worth);
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ItemEquipped> domainEvent)
    {
        var id = domainEvent.AggregateEvent.ItemId;
        var item = Inventory.First(i => i.Id == id);
        item.Equip();
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ItemUnequipped> domainEvent)
    {
        var id = domainEvent.AggregateEvent.ItemId;
        var item = Inventory.First(i => i.Id == id);
        item.Unequip();
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, LevelGained> domainEvent)
    {
        Level = domainEvent.AggregateEvent.NewLevel;
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, WoundsChanged> domainEvent)
    {
        Wounds = domainEvent.AggregateEvent.NewWounds;
    }

    public string Id { get; private set; }
    public long? Version { get; set; }
}