using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class ShopDeleted : DomainEvent
{
    public ShopDeleted(Shop shop)
    {
        Shop = shop;
    }

    public Shop Shop { get; private set; }
}