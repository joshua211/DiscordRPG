using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild;

namespace DiscordRPG.Client.Dialogs;

public class BuyDialog : Dialog
{
    public BuyDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }

    public GuildId GuildId { get; set; }
    public CharacterId CharacterId { get; set; }
    public List<Item> AvailableItems { get; set; }
    public Item? SelectedItem { get; set; }
    public IEnumerable<Item> EquippedItems { get; set; }
    public int Money { get; set; }
}