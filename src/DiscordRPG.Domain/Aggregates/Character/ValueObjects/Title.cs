using DiscordRPG.Domain.Enums;
using EventFlow.Core;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Aggregates.Character.ValueObjects;

public class Title : Entity<TitleId>
{
    public Title(TitleId id, Rarity rarity, string name, IEnumerable<StatusEffect> effects, bool isEquipped) : base(id)
    {
        Rarity = rarity;
        Name = name;
        Effects = effects;
        IsEquipped = isEquipped;
    }

    public Rarity Rarity { get; private set; }
    public string Name { get; private set; }
    public IEnumerable<StatusEffect> Effects { get; private set; }
    public bool IsEquipped { get; private set; }

    public void Equip()
    {
        IsEquipped = true;
    }

    public void Unequip()
    {
        IsEquipped = false;
    }

    public override string ToString()
    {
        return $"[{Rarity.ToString()}] {Name}";
    }
}

public class TitleId : Identity<TitleId>
{
    public TitleId(string value) : base(value)
    {
    }
}