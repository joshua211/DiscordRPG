namespace DiscordRPG.Application.Data;

public static class Aspects
{
    public static Dictionary<Rarity, List<Aspect>> AspectsByRarity;
    public static Aspect CraftedAspect;
    public static Aspect DebugAspect;
    public static Aspect OrdinaryAspect;
    private static IEnumerable<string> commonDefault = new[] {"Big", "Small", "Old", "Rusty", "Normal"};
    private static IEnumerable<string> uncommonDefault = new[] {"Great", "Good", "Sharp"};
    private static IEnumerable<string> rareDefault = new[] {"Grand", "Rare", "Shining", "Sharp", "Rare"};
    private static IEnumerable<string> uniqueDefault = new[] {"Ancient", "Unique", "Powerful", "Grand"};
    private static IEnumerable<string> legendaryDefault = new[] {"Legendary"};
    private static IEnumerable<string> mythicDefault = new[] {"Mythic"};

    static Aspects()
    {
        CraftedAspect = new Aspect("Crafted", new()
        {
            {Rarity.Common, new[] {"Crafted"}},
            {Rarity.Uncommon, new[] {"Crafted"}},
            {Rarity.Rare, new[] {"Crafted"}},
            {Rarity.Unique, new[] {"Crafted"}},
            {Rarity.Legendary, new[] {"Crafted"}},
            {Rarity.Mythic, new[] {"Crafted"}},
        });
        DebugAspect = new Aspect("DEBUG", new()
        {
            {Rarity.Common, new[] {"DEBUG"}},
            {Rarity.Uncommon, new[] {"DEBUG"}},
            {Rarity.Rare, new[] {"DEBUG"}},
            {Rarity.Unique, new[] {"DEBUG"}},
            {Rarity.Legendary, new[] {"DEBUG"}},
            {Rarity.Mythic, new[] {"DEBUG"}},
            {Rarity.Divine, new[] {"DEBUG"}},
        });
        OrdinaryAspect = new Aspect("Ordinary", new()
        {
            {Rarity.Common, new[] {"Ordinary"}},
            {Rarity.Uncommon, new[] {"Ordinary"}},
            {Rarity.Rare, new[] {"Ordinary"}}
        });

        AspectsByRarity = new Dictionary<Rarity, List<Aspect>>()
        {
            {
                Rarity.Common, new List<Aspect>()
                {
                    new("Dark", new()
                    {
                        {Rarity.Common, commonDefault.Append("Dark")}
                    }),
                    new("Foul", new()
                    {
                        {Rarity.Common, commonDefault.Append("Foul")}
                    }),
                    new("Big", new()
                    {
                        {Rarity.Common, commonDefault.Append("Big")}
                    }),
                    new("Dirty", new()
                    {
                        {Rarity.Common, commonDefault.Append("Dirty")}
                    }),
                    new("Old", new()
                    {
                        {Rarity.Common, commonDefault.Append("Old")}
                    }),
                    new("Rusty", new()
                    {
                        {Rarity.Common, commonDefault.Append("Rusty")}
                    }),
                    new("Green", new()
                    {
                        {Rarity.Common, commonDefault.Append("Green")}
                    }),
                    new("Red", new()
                    {
                        {Rarity.Common, commonDefault.Append("Red")}
                    }),
                    new("Blue", new()
                    {
                        {Rarity.Common, commonDefault.Append("Blue")}
                    }),
                }
            },
            {
                Rarity.Uncommon, new List<Aspect>()
                {
                    new("Great", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault}
                    }),
                    new("Vast", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault}
                    }),
                    new("Old", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault}
                    }),
                    new("Hot", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault.Append("Hot")}
                    }),
                    new("Cold", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault.Append("Cold")}
                    }),
                    new("Creepy", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault.Append("Creepy")}
                    }),
                    new("Dreary", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault.Append("Dreary")}
                    }),
                }
            },
            {
                Rarity.Rare, new List<Aspect>()
                {
                    new("Grand", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault}
                    }),
                    new("Ancient", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault.Append("Ancient")}
                    }),
                    new("Silent", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault.Append("Silent")}
                    }),
                    new("Vile", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault.Append("Sinister")}
                    }),
                    new("Bloody", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault.Append("Bloody")}
                    }),
                }
            },
            {
                Rarity.Unique, new List<Aspect>()
                {
                    new("Ancient", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault}
                    }),
                    new("Grand", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Grand")}
                    }),
                    new("Horrid", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Horrible")}
                    }),
                    new("Frightful", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Frightful").Append("Evil")}
                    }),
                    new("Cloudy", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Cloudy")}
                    }),
                    new("Blazing", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Blazing").Append("Fire")}
                    }),
                    new("Freezing", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uniqueDefault.Append("Freezing").Append("Ice")}
                    })
                }
            },
            {
                Rarity.Legendary, new List<Aspect>()
                {
                    new("Imperial", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault.Append("Imperial")}
                    }),
                    new("Infernal", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault.Append("Infernal")}
                    }),
                    new("Royal", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault.Append("Royal")}
                    }),
                    new("Void", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault.Append("Void")}
                    })
                }
            },
            {
                Rarity.Mythic, new List<Aspect>()
                {
                    new("Hellish", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault.Append("Hellish")}
                    }),
                    new("Eternal", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault.Append("Eternal")}
                    }),
                    new("Icebound", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault.Append("Icebound")}
                    }),
                    new("Everlasting", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault.Append("Everlasting")}
                    }),
                    new("Chaos", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault.Append("Chaos")}
                    }),
                }
            },
            {
                Rarity.Divine, new List<Aspect>()
                {
                    new("Divine", new()
                    {
                        {Rarity.Common, commonDefault},
                        {Rarity.Uncommon, uncommonDefault},
                        {Rarity.Rare, rareDefault},
                        {Rarity.Unique, uncommonDefault},
                        {Rarity.Legendary, legendaryDefault},
                        {Rarity.Mythic, mythicDefault},
                        {Rarity.Divine, new[] {"Divine", "Celestial", "Godly"}}
                    }),
                }
            }
        };
    }
}