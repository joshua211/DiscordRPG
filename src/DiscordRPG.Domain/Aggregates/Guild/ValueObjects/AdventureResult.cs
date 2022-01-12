using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

public class AdventureResult : ValueObject
{
    public List<Wound> Wounds { get; init; }
    public List<Item> Items { get; private set; }
    public List<Encounter> Encounters { get; private set; }
    public ulong Experience { get; init; }
}