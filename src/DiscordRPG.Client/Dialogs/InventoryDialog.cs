using DiscordRPG.Application.Models;
using DiscordRPG.Common;

namespace DiscordRPG.Client.Dialogs;

public class InventoryDialog : Dialog
{
    public InventoryDialog(ulong userId, TransactionContext context) : base(userId, context)
    {
    }


    public CharacterReadModel Character { get; set; }
    public int CurrentPage { get; set; }
    public string CurrentCategory { get; set; }
    public int MaxPagesOfCurrentCategory { get; set; }
}