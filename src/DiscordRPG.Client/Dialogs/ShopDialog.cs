using DiscordRPG.Application.Models;
using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class ShopDialog : Dialog
{
    public ShopDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public bool IsBuying { get; set; }
    public GuildId GuildId { get; set; }
    public CharacterReadModel Character { get; set; }
    public List<Item> PlayerShop { get; set; }

    public Item SelectedItem { get; set; }

    /*public Shop GuildShop { get; set; }*/
    public int CurrentPage { get; set; } = 1;
}