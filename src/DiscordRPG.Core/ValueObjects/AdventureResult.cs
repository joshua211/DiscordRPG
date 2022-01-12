using DiscordRPG.Core.DomainServices;

namespace DiscordRPG.Core.ValueObjects;

public class AdventureResult
{
    public AdventureResult(List<Wound> wounds, List<Item> items, ulong experience, List<Encounter> encounters)
    {
        Wounds = wounds;
        Items = items;
        Experience = experience;
        Encounters = encounters;
    }

    public List<Wound> Wounds { get; init; }
    public List<Item> Items { get; private set; }
    public List<Encounter> Encounters { get; private set; }
    public ulong Experience { get; init; }
}