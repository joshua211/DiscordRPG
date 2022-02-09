using EventFlow.Core;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Character;

public class CharacterId : SingleValueObject<string>, IIdentity
{
    public CharacterId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is CharacterId id)
            return id.Value == Value;

        return false;
    }
}