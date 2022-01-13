using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Data;

public static class Items
{
    public static Dictionary<Rarity, List<(string name, string descr)>> ItemNamesByRarity;

    static Items()
    {
        ItemNamesByRarity = new Dictionary<Rarity, List<(string name, string descr)>>
        {
            {
                Rarity.Common, new List<(string name, string descr)>
                {
                    ("Hide", "The hide of some animal"),
                    ("Ore", "A common ore often found in mines"),
                    ("Bone", "The remains of some animal"),
                    ("Herb", "A bitter herb often used to cure illness")
                }
            },
            {
                Rarity.Uncommon, new List<(string name, string descr)>
                {
                    ("Hide", "The hide of some animal"),
                    ("Ore", "A common ore often found in mines"),
                    ("Bone", "The remains of some animal"),
                    ("Herb", "A bitter herb often used to cure illness")
                }
            },
            {
                Rarity.Rare, new List<(string name, string descr)>
                {
                    ("Scale", "A hard and durable scale"),
                    ("Crystal", "A rare crystal that can be used in crafting"),
                    ("Blood", "Some strong monsters have equally strong blood"),
                    ("Medical Plant", "A rare plant that only grows in remote places")
                }
            },
            {
                Rarity.Unique, new List<(string name, string descr)>
                {
                    ("Scale", "A hard and durable scale"),
                    ("Crystal", "A rare crystal that can be used in crafting"),
                    ("Blood", "Some strong monsters have equally strong blood"),
                    ("Medical Plant", "A rare plant that only grows in remote places")
                }
            },
            {
                Rarity.Legendary, new List<(string name, string descr)>
                {
                    ("Ancient Hide", "The rare skin of a powerful beast"),
                    ("Ancient Ore", "A rare ore that can be forged into legendary items"),
                    ("Ancient Marrow", "The bone marrow of a powerful beast"),
                    ("Alchemical Substance", "A rare substance used by mystical alchemists")
                }
            },
            {
                Rarity.Mythic, new List<(string name, string descr)>
                {
                    ("Ancient Hide", "The rare skin of a powerful beast"),
                    ("Ancient Ore", "A rare ore that can be forged into legendary items"),
                    ("Ancient Marrow", "The bone marrow of a powerful beast"),
                    ("Alchemical Substance", "A rare substance used by mystical alchemists")
                }
            },
            {
                Rarity.Divine, new List<(string name, string descr)>
                {
                    ("Sacred shard", "A shard of some unknown entity")
                }
            },
        };
    }
}