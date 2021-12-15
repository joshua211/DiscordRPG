using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Dialogs;

public class InventoryDialog : Dialog
{
    public InventoryDialog()
    {
    }

    public InventoryDialog(ulong userId) : base(userId)
    {
    }

    public Character Character { get; set; }
    public int CurrentPage { get; set; }
    public string CurrentCategory { get; set; }
    public int MaxPagesOfCurrentCategory { get; set; }
}