namespace DiscordRPG.Core.ValueObjects;

public class DungeonResult
{
    public DungeonResult(List<Wound> wounds, List<Item> items)
    {
        Wounds = wounds;
        Items = items;
    }

    public List<Wound> Wounds { get; init; }
    public List<Item> Items { get; private set; }
}