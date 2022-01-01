using DiscordRPG.Core.Entities;
using DiscordRPG.Core.ValueObjects;

namespace DiscordRPG.Client.Dialogs;

public class UseItemDialog : Dialog
{
    public UseItemDialog()
    {
    }

    public UseItemDialog(ulong userId) : base(userId)
    {
    }

    public Character Character { get; set; }
    public Item? SelectedItem { get; set; }
}