using EventFlow.Core;

namespace DiscordRPG.Domain.Entities.Shop;

public class ShopId : Identity<ShopId>
{
    public ShopId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }
}