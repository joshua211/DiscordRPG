using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

public class AdventureResult : ValueObject
{
    public AdventureResult(List<Wound> wounds, List<Item> items, List<Encounter> encounters, ulong experience)
    {
        Wounds = wounds;
        Items = items;
        Encounters = encounters;
        Experience = experience;
    }

    public List<Wound> Wounds { get; init; }
    public List<Item> Items { get; private set; }
    public List<Encounter> Encounters { get; private set; }
    public ulong Experience { get; init; }
}