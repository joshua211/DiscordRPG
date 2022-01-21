using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class SellDialog : Dialog, IPageableDialog
{
    public SellDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public GuildId GuildId { get; set; }
    public CharacterId CharacterId { get; set; }
    public List<Item> SellableItems { get; set; }
    public Item? SelectedItem { get; set; }
    public int CurrentMoney { get; set; }
    public int CurrentPage { get; set; } = 1;
}