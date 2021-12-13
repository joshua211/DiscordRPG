namespace DiscordRPG.Application.Data;

public static class Aspects
{
    public static Dictionary<Rarity, List<Aspect>> AspectsByRarity;

    static Aspects()
    {
        AspectsByRarity = new Dictionary<Rarity, List<Aspect>>()
        {
            {
                Rarity.Common, new List<Aspect>()
                {
                    new("Dark", new[] {"Big", "Small", "Old", "Dark", "Rusty"}),
                    new("Foul", new[] {"Big", "Small", "Old", "Foul", "Rusty"}),
                    new("Big", new[] {"Big", "Old", "Rusty"}),
                    new("Dirty", new[] {"Big", "Small", "Old", "Dirty", "Rusty"}),
                    new("Old", new[] {"Big", "Small", "Old", "Rusty"}),
                    new("Rusty", new[] {"Big", "Small", "Rusty", "Rusty"}),
                    new("Green", new[] {"Green"}),
                    new("Red", new[] {"Red"}),
                    new("Blue", new[] {"Blue"}),
                }
            },
            {
                Rarity.Uncommon, new List<Aspect>()
                {
                    new("Great", new[] {"Great", "Good", "Sharp"}),
                    new("Vast", new[] {"Great", "Good", "Sharp"}),
                    new("Old", new[] {"Great", "Good", "Old"}),
                    new("Cold", new[] {"Great", "Good", "Sharp", "Cold"}),
                    new("Hot", new[] {"Great", "Good", "Sharp", "Hot"}),
                    new("Creepy", new[] {"Great", "Good", "Sharp"}),
                    new("Dreary", new[] {"Great", "Good", "Sharp"}),
                }
            },
            {
                Rarity.Rare, new List<Aspect>()
                {
                    new("Grand", new[] {"Grand", "Rare", "Shining", "Sharp", "Rare"}),
                    new("Ancient", new[] {"Grand", "Rare", "Shining", "Sharp", "Ancient", "Rare"}),
                    new("Grand", new[] {"Grand", "Rare", "Shining", "Sharp", "Rare"}),
                    new("Silent", new[] {"Grand", "Rare", "Shining", "Sharp"}),
                    new("Vile", new[] {"Grand", "Rare", "Shining", "Sharp", "Vile"}),
                    new("Sinister", new[] {"Grand", "Rare", "Shining", "Sharp", "Sinister"}),
                    new("Bloody", new[] {"Ancient", "Unique", "Powerful", "Bloody"}),
                }
            },
            {
                Rarity.Unique, new List<Aspect>()
                {
                    new("Ancient", new[] {"Ancient", "Unique", "Powerful"}),
                    new("Grand", new[] {"Ancient", "Unique", "Powerful", "Grand"}),
                    new("Horrid", new[] {"Ancient", "Unique", "Powerful", "Horrible"}),
                    new("Frightful", new[] {"Ancient", "Unique", "Powerful", "Evil"}),
                    new("Cloudy", new[] {"Ancient", "Unique", "Powerful", "Cloud"}),
                    new("Blazing", new[] {"Ancient", "Unique", "Powerful", "Blazing", "Fire"}),
                    new("Freezing", new[] {"Ancient", "Unique", "Powerful", "Freezing", "Ice"}),
                }
            },
            {
                Rarity.Legendary, new List<Aspect>()
                {
                    new("Imperial", new[] {"Imperial", "Legendary"}),
                    new("Infernal", new[] {"Infernal", "Legendary"}),
                    new("Infernal", new[] {"Royal", "Legendary"}),
                    new("Void", new[] {"Void", "Legendary"}),
                }
            },
            {
                Rarity.Mythic, new List<Aspect>()
                {
                    new("Hellish", new[] {"Hellish", "Mythic"}),
                    new("Eternal", new[] {"Eternal", "Mythic"}),
                    new("Icebound", new[] {"Icebound", "Mythic"}),
                    new("Everlasting", new[] {"Everlasting", "Mythic"}),
                    new("Chaos", new[] {"Chaos", "Mythic"}),
                }
            },
            {
                Rarity.Divine, new List<Aspect>()
                {
                    new("Divine", new[] {"Divine", "Divine", "Celestial", "Godly"}),
                }
            }
        };
    }
}