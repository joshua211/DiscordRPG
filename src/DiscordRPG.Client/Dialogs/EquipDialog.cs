using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class EquipDialog : Dialog
{
    public EquipDialog()
    {
    }

    public EquipDialog(ulong userId) : base(userId)
    {
    }

    public Character Character { get; set; }
    public Item CurrentItem { get; set; }
}