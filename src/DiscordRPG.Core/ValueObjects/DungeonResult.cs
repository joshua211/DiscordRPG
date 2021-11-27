namespace DiscordRPG.Core.ValueObjects;

public class DungeonResult
{
    public DungeonResult(List<Wound> wounds)
    {
        Wounds = wounds;
    }

    public List<Wound> Wounds { get; init; }
}