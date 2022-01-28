using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

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