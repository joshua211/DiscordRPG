using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Level
{
    [BsonConstructor]
    public Level(uint currentLevel, int currentExp, int needExp)
    {
        CurrentLevel = currentLevel;
        CurrentExp = currentExp;
        NeedExp = needExp;
    }

    public uint CurrentLevel { get; private set; }
    public int CurrentExp { get; private set; }
    public int NeedExp { get; private set; }
}