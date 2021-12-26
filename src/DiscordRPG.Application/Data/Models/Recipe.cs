namespace DiscordRPG.Application.Data.Models;

public class Recipe
{
    public Recipe(int resource1, int resource2, int resource3, int resource4, Rarity rarity, uint level, string name)
    {
        Resource1 = resource1;
        Resource2 = resource2;
        Resource3 = resource3;
        Resource4 = resource4;
        Rarity = rarity;
        Level = level;
        Name = name;
    }

    public int Resource1 { get; private set; }
    public int Resource2 { get; private set; }
    public int Resource3 { get; private set; }
    public int Resource4 { get; private set; }
    public Rarity Rarity { get; private set; }
    public uint Level { get; private set; }
    public string Name { get; private set; }
}