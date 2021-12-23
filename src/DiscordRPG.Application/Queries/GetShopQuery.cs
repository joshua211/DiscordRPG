using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetShopQuery : Query<Shop>
{
    public GetShopQuery(Identity shopId)
    {
        ShopId = shopId;
    }

    public Identity ShopId { get; private set; }
}