using DiscordRPG.Core.Entities;

namespace DiscordRPG.Client.Dialogs;

public class CraftingDialog : Dialog
{
    public CraftingDialog(ulong id) : base(id)
    {
    }

    public CraftingDialog()
    {
    }

    public Character Character { get; set; }
}