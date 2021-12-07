namespace DiscordRPG.Core.ValueObjects;

public class AdventureResult
{
    public AdventureResult(List<Wound> wounds, List<Item> items, ulong experience)
    {
        Wounds = wounds;
        Items = items;
        Experience = experience;
    }

    public List<Wound> Wounds { get; init; }
    public List<Item> Items { get; private set; }
    public ulong Experience { get; init; }
}