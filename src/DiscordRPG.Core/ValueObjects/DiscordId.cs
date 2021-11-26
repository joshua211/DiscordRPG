namespace DiscordRPG.Core.ValueObjects;

public class DiscordId
{
    public DiscordId(ulong value)
    {
        Value = value;
    }

    public ulong Value { get; private set; }

    public static implicit operator ulong(DiscordId id) => id.Value;
    public static implicit operator DiscordId(ulong val) => new DiscordId(val);

    public override string ToString()
    {
        return Value.ToString();
    }
}