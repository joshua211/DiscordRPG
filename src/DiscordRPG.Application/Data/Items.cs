using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Data;

public static class Items
{
    public static Dictionary<Rarity, List<(string name, string descr, CharacterAttribute attribute)>> ItemNamesByRarity;

    static Items()
    {
        ItemNamesByRarity = new Dictionary<Rarity, List<(string name, string descr, CharacterAttribute attribute)>>
        {
            {
                Rarity.Common, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Hide", "The hide of some animal", CharacterAttribute.Agility),
                    ("Ore", "A common ore often found in mines", CharacterAttribute.Strength),
                    ("Bone", "The remains of some animal", CharacterAttribute.Vitality),
                    ("Herb", "A bitter herb often used to cure illness", CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Uncommon, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Hide", "The hide of some animal", CharacterAttribute.Agility),
                    ("Ore", "A common ore often found in mines", CharacterAttribute.Strength),
                    ("Bone", "The remains of some animal", CharacterAttribute.Vitality),
                    ("Herb", "A bitter herb often used to cure illness", CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Rare, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Scale", "A hard and durable scale", CharacterAttribute.Agility),
                    ("Crystal", "A rare crystal that can be used in crafting", CharacterAttribute.Strength),
                    ("Blood", "Some strong monsters have equally strong blood", CharacterAttribute.Vitality),
                    ("Medical Plant", "A rare plant that only grows in remote places", CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Unique, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Scale", "A hard and durable scale", CharacterAttribute.Agility),
                    ("Crystal", "A rare crystal that can be used in crafting", CharacterAttribute.Strength),
                    ("Blood", "Some strong monsters have equally strong blood", CharacterAttribute.Vitality),
                    ("Medical Plant", "A rare plant that only grows in remote places", CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Legendary, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Ancient Hide", "The rare skin of a powerful beast", CharacterAttribute.Agility),
                    ("Ancient Ore", "A rare ore that can be forged into legendary items", CharacterAttribute.Strength),
                    ("Ancient Marrow", "The bone marrow of a powerful beast", CharacterAttribute.Vitality),
                    ("Alchemical Substance", "A rare substance used by mystical alchemists",
                        CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Mythic, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Ancient Hide", "The rare skin of a powerful beast", CharacterAttribute.Agility),
                    ("Ancient Ore", "A rare ore that can be forged into legendary items", CharacterAttribute.Strength),
                    ("Ancient Marrow", "The bone marrow of a powerful beast", CharacterAttribute.Vitality),
                    ("Alchemical Substance", "A rare substance used by mystical alchemists",
                        CharacterAttribute.Intelligence)
                }
            },
            {
                Rarity.Divine, new List<(string name, string descr, CharacterAttribute attribute)>
                {
                    ("Sacred shard", "A shard of some unknown entity", CharacterAttribute.Luck)
                }
            },
        };
    }
}