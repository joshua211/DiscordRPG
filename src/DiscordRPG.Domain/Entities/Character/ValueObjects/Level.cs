using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Level : ValueObject
{
    public Level(uint currentLevel, ulong currentExp, ulong requiredExp)
    {
        if (currentExp >= requiredExp)
            DomainError.With(nameof(currentExp), nameof(requiredExp));

        CurrentLevel = currentLevel;
        CurrentExp = currentExp;
        RequiredExp = requiredExp;
    }

    public uint CurrentLevel { get; set; }
    public ulong CurrentExp { get; set; }
    public ulong RequiredExp { get; set; }
}