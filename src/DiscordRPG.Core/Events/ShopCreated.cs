using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class ShopCreated : DomainEvent
{
    public ShopCreated(Shop shop)
    {
        Shop = shop;
    }

    public Shop Shop { get; private set; }
}