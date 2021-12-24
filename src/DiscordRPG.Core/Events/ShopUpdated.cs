using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class ShopUpdated : DomainEvent
{
    public ShopUpdated(Shop shop)
    {
        Shop = shop;
    }

    public Shop Shop { get; private set; }
}