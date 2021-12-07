using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Level
{
    [BsonConstructor]
    public Level(uint currentLevel, ulong currentExp, ulong requiredExp)
    {
        CurrentLevel = currentLevel;
        CurrentExp = currentExp;
        RequiredExp = requiredExp;
    }

    public uint CurrentLevel { get; set; }
    public ulong CurrentExp { get; set; }
    public ulong RequiredExp { get; set; }
}