using DiscordRPG.Common;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class ShopDialog : Dialog
{
    public ShopDialog()
    {
    }

    public ShopDialog(ulong userId) : base(userId)
    {
    }

    public bool IsBuying { get; set; }
    public Identity GuildId { get; set; }
    public Character Character { get; set; }
    public List<Equipment> PlayerShop { get; set; }
    public Item SelectedItem { get; set; }
    public Shop GuildShop { get; set; }
    public int CurrentPage { get; set; }
}