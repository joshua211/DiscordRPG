namespace DiscordRPG.Core.Events;

public class LevelGained : DomainEvent
{
    public LevelGained(uint oldLevel, uint newLevel, Identity charId)
    {
        OldLevel = oldLevel;
        NewLevel = newLevel;
        CharId = charId;
    }

    public uint OldLevel { get; private set; }
    public uint NewLevel { get; private set; }
    public Identity CharId { get; private set; }
}